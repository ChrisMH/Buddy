using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Buddy.Utility;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace Buddy.Web.TabularQuery
{
    public static class QueryableExtensions
    {
        public static TabularResponse<T> ApplyQuery<T>(this IQueryable<T> data, TabularQuery query)
        {
            var result = new TabularResponse<T>();

            // Filter 
            data = data.Filter(query.Filter);

            // Count and aggregate
            result.Count = data.Count();
            result.Aggregates = data.Aggregate(query.Aggregate);

            // Sort
            data = data.Sort(query.Sort);

            // Page
            data = data.Page(query.Skip, query.Take);

            result.Data = data.ToList();

            return result;
        }

        public static object Aggregate<T>(this IQueryable<T> data, IEnumerable<AggregateExpression> aggregate)
        {
            if (aggregate == null || !aggregate.Any() || !data.Any())
                return null;

            var aggregateObjectProps = new Dictionary<DynamicProperty, object>();

            var aggregateGroups = aggregate.GroupBy(a => a.Field);
            foreach (var aggregateGroup in aggregateGroups)
            {
                var fieldProperty = typeof(T).GetProperty(aggregateGroup.Key.ToUpperCamelCase());
                var aggregateFieldProps = new Dictionary<DynamicProperty, object>();

                foreach (var aggregateExp in aggregateGroup)
                {
                    var aggregateMethod = aggregateExp.AggregateMethod(typeof(T));
                    if (aggregateMethod == null)
                    {
                        aggregateFieldProps.Add(new DynamicProperty(aggregateExp.Aggregate, typeof(object)), 0);
                        continue;
                    }
                    
                    var expParam = Expression.Parameter(typeof(T), "p");

                    // Field access lambda expression.  For count types that are nullable, use a selector that checks for not null
                    var fieldAccessExpression =
                        aggregateExp.Aggregate == AggregateExpression.Count && Nullable.GetUnderlyingType(fieldProperty.PropertyType) != null ?
                        Expression.Lambda(Expression.NotEqual(Expression.MakeMemberAccess(expParam, fieldProperty), Expression.Constant(null, fieldProperty.PropertyType)), expParam) :
                        Expression.Lambda(Expression.MakeMemberAccess(expParam, fieldProperty), expParam);

                    // Expression caller.  Non-nullable count only requires the data expression, otherwise
                    // send the data expression and the field access expression.
                     var call = Expression.Call(null, aggregateMethod, 
                         aggregateExp.Aggregate == AggregateExpression.Count && Nullable.GetUnderlyingType(fieldProperty.PropertyType) == null ?
                            new [] { data.Expression } :
                            new [] { data.Expression, fieldAccessExpression });

                    // Execute the expression and store the value
                    var value = data.Provider.Execute(call);
                    aggregateFieldProps.Add(new DynamicProperty(aggregateExp.Aggregate, typeof(object)), value);
                }
                
                // Create the field-level aggregate result object
                var aggregateFieldObjectType = DynamicExpression.CreateClass(aggregateFieldProps.Keys);
                var aggregateFieldObject = Activator.CreateInstance(aggregateFieldObjectType);
                foreach(var prop in aggregateFieldProps.Keys)
                    aggregateFieldObjectType.GetProperty(prop.Name).SetValue(aggregateFieldObject, aggregateFieldProps[prop]);
                aggregateObjectProps.Add(new DynamicProperty(aggregateGroup.Key, aggregateFieldObjectType), aggregateFieldObject);
            }
            
            // Create the top-level aggregate result object
            var aggregateObjectType = DynamicExpression.CreateClass(aggregateObjectProps.Keys);
            var aggregateObject = Activator.CreateInstance(aggregateObjectType);
            foreach(var prop in aggregateObjectProps.Keys)
                aggregateObjectType.GetProperty(prop.Name).SetValue(aggregateObject, aggregateObjectProps[prop]);

            return aggregateObject;
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> data, FilterExpression filter)
        {
            if (filter == null)
                return data;
            
            var param = new List<object>();
            var expression = filter.ToExpression<T>(param);

            if (string.IsNullOrWhiteSpace(expression))
                return data;
            
            return data.Where(expression, param.ToArray());
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> data, IEnumerable<SortExpression> sort)
        {
            if (sort == null || !sort.Any())
                return data;

            var sortExpression = string.Join(",", sort.Select(s => s.ToDynamicLinqExpression()));

            return data.OrderBy(sortExpression);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> data, int skip, int take)
        {
            return take > 0 ? data.Skip(skip).Take(take) : data;
        }
    }
}