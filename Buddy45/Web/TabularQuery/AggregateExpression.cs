using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Buddy.Utility;

namespace Buddy.Web.TabularQuery
{/// <summary>
    /// AggregateExpression part of a tabular query
    /// The query string part looks like: aggregate[0].field=backlog&aggregate[0].aggregate=average&aggregate[1].field=totalReceived&aggregate[1].aggregate=sum ...
    /// Supported aggregate values:
    ///   average
    ///   count
    ///   max
    ///   min
    ///   sum
    /// </summary>
    public class AggregateExpression
    {
        public const string Average = "average";
        public const string Count = "count";
        public const string Max = "max";
        public const string Min = "min";
        public const string Sum = "sum";

        public string Field { get; set; }
        public string Aggregate { get; set; }

        public static readonly IDictionary<string, string> Aggregates = new Dictionary<string, string>
        {
            {Average, "Average"},
            {Count, "Count"},
            {Max, "Max"},
            {Min, "Min"},
            {Sum, "Sum"},
        };

        /// <summary>
        /// Convert to an expression suitable for calling by dynamic linq.
        /// Note that the assumption is that this will be called as part of a grouping so that the
        /// aggregate functions will work across the entire group like:
        /// data.GroupBy(g => 1).Select(ToExpression(...))
        /// </summary>
        /// <param name="aggregates"></param>
        /// <returns></returns>
        public static string ToExpression(IEnumerable<AggregateExpression> aggregates)
        {
            var aggregatesByField = aggregates.GroupBy(a => a.Field);
            var fieldExpressions = aggregatesByField.Select(g => ToFieldExpression(g.ToList())).ToList();
            if (!fieldExpressions.Any())
                return null;

            return $"new ({fieldExpressions.Aggregate((c, n) => string.Concat(c, ", ", n))})";
        }

        /// <summary>
        /// Generate aggregates for an individual field
        /// </summary>
        /// <param name="aggregates"></param>
        /// <returns></returns>
        private static string ToFieldExpression(List<AggregateExpression> aggregates)
        {
            if (aggregates == null || !aggregates.Any())
                return null;

            var expressions = aggregates.Select(ToAggregateExpression).Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
            if (!expressions.Any())
                return null;

            return $"new ({expressions.Aggregate((c, n) => string.Concat(c, ",", n))}) as {aggregates.First().Field}";
        }

        /// <summary>
        /// Generate an individual aggregate for a field
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        private static string ToAggregateExpression(AggregateExpression aggregate)
        {
            if (aggregate == null || string.IsNullOrWhiteSpace(aggregate.Field) || string.IsNullOrWhiteSpace(aggregate.Aggregate))
                return null;

            if(aggregate.Aggregate == AggregateExpression.Count)
                return $"it.{AggregateExpression.Aggregates[aggregate.Aggregate]}() as {aggregate.Aggregate}";
            return $"{AggregateExpression.Aggregates[aggregate.Aggregate]}(it.{aggregate.Field.ToUpperCamelCase()}) as {aggregate.Aggregate}";
        }

        
        /* This version is slightly slower and much more complicated.  Keeping it around in case it's useful someday since
         * it was a pain in the ass to write.
         * 
        /// <summary>
        /// Retrieves the appropriate aggreggate method for this aggregate expression
        /// </summary>
        /// <param name="sourceType">The type of object containing the field that will be aggregated</param>
        /// <returns>The aggregate method, null if not found</returns>
        public MethodInfo AggregateMethod(Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            var fieldType = sourceType.GetProperty(Field.ToUpperCamelCase()).PropertyType;
            if(fieldType == null)
                throw new Exception($"Source type '{sourceType.Name}' does not contain a field named '{Field.ToUpperCamelCase()}'");
            
            switch (Aggregate)
            {
                case Count:
                {
                    var getParameterTypes = Nullable.GetUnderlyingType(fieldType) != null ? CountNullableParameterTypes() : CountParameterTypes();
                    var aggregateMethod = GetMethod(Aggregates[Aggregate], getParameterTypes, 1);
                    if (aggregateMethod != null)
                        return aggregateMethod.MakeGenericMethod(sourceType);
                    return null;
                };

                case Average:
                case Sum:
                {
                    var getParameterTypes = AverageSumParameterTypes(fieldType);
                    var aggregateMethod = GetMethod(Aggregates[Aggregate], getParameterTypes, 1);
                    if (aggregateMethod != null)
                        return aggregateMethod.MakeGenericMethod(sourceType);
                    return null;
                }

                case Min:
                case Max:
                {
                    var getParameterTypes = MinMaxParameterTypes();
                    var aggregateMethod = GetMethod(Aggregates[Aggregate], getParameterTypes, 2);
                    if (aggregateMethod != null)
                        return aggregateMethod.MakeGenericMethod(sourceType, fieldType);
                    return null;
                }

                default:
                    return null;
            }
        }

        /// <summary>
        /// Searches available Queryable methods for one that fits the criteria
        /// </summary>
        /// <param name="methodName">The method names</param>
        /// <param name="getParameterTypes">Required parameter methods</param>
        /// <param name="genericArgumentCount">Number of generic arguments for the method</param>
        /// <returns></returns>
        private static MethodInfo GetMethod(string methodName, Func<Type[], Type[]> getParameterTypes , int genericArgumentCount)
        {
            var methods = from method in typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                let parameters = method.GetParameters()
                let genericArguments = method.GetGenericArguments()
                where method.Name == methodName &&
                      genericArguments.Length == genericArgumentCount &&
                      parameters.Select(p => p.ParameterType).SequenceEqual(getParameterTypes.Invoke(genericArguments))
                select method;
            return methods.FirstOrDefault();
        }
        
        private static Func<Type[], Type[]> AverageSumParameterTypes(Type fieldType)
		{
			return genericArguments => new[]
				{
					typeof (IQueryable<>).MakeGenericType(genericArguments[0]),
					typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(genericArguments[0], fieldType))
				};
		}

		private static Func<Type[], Type[]> CountNullableParameterTypes()
		{
			return genericArguments => new[]
				{
					typeof(IQueryable<>).MakeGenericType(genericArguments[0]),
					typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(genericArguments[0], typeof(bool)))
				};
		}

		private static Func<Type[], Type[]> CountParameterTypes()
		{
			return genericArguments => new[]
				{
					typeof(IQueryable<>).MakeGenericType(genericArguments[0])
				};
		}

		private static Func<Type[], Type[]> MinMaxParameterTypes()
		{
			return genericArguments => new[]
				{
					typeof (IQueryable<>).MakeGenericType(genericArguments[0]),
					typeof (Expression<>).MakeGenericType(typeof (Func<,>).MakeGenericType(genericArguments[0], genericArguments[1]))
				};
		}

        */
    }
}