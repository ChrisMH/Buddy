using System;
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
        public static string Average = "average";
        public static string Count = "count";
        public static string Max = "max";
        public static string Min = "min";
        public static string Sum = "sum";

        public string Field { get; set; }
        public string Aggregate { get; set; }

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
                case "count":
                {
                    var getParameterTypes = Nullable.GetUnderlyingType(fieldType) != null ? CountNullableParameterTypes() : CountParameterTypes();
                    var aggregateMethod = GetMethod(Aggregate.ToUpperCamelCase(), getParameterTypes, 1);
                    if (aggregateMethod != null)
                        return aggregateMethod.MakeGenericMethod(sourceType);
                    return null;
                };

                case "average":
                case "sum":
                {
                    var getParameterTypes = AverageSumParameterTypes(fieldType);
                    var aggregateMethod = GetMethod(Aggregate.ToUpperCamelCase(), getParameterTypes, 1);
                    if (aggregateMethod != null)
                        return aggregateMethod.MakeGenericMethod(sourceType);
                    return null;
                }

                case "min":
                case "max":
                {
                    var getParameterTypes = MinMaxParameterTypes();
                    var aggregateMethod = GetMethod(Aggregate.ToUpperCamelCase(), getParameterTypes, 2);
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
    }
}