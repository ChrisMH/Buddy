using System;
using System.Collections.Generic;
using System.Linq;
using Buddy.Utility;

namespace Buddy.Web.TabularQuery
{
    public class FilterExpression
    {
        public const string Eq = "eq";
        public const string Neq = "neq";
        public const string Lt = "lt";
        public const string Lte = "lte";
        public const string Gt = "gt";
        public const string Gte = "gte";
        public const string StartsWith = "startswith";
        public const string EndsWith = "endswith";
        public const string Contains = "contains";
        public const string DoesNotContain = "doesnotcontain";
        public const string IsNull = "isnull";
        public const string IsNotNull = "isnotnull";
        public const string IsEmpty = "isempty";
        public const string IsNotEmpty = "isnotempty";

        protected readonly IDictionary<string, string> Operators = new Dictionary<string, string>
        {
            {Eq, "="},
            {Neq, "!="},
            {Lt, "<"},
            {Lte, "<="},
            {Gt, ">"},
            {Gte, ">="},
            {StartsWith, "StartsWith"},
            {EndsWith, "EndsWith"},
            {Contains, "Contains"},
            {DoesNotContain, "Contains"},
            {IsNull, "= null"},
            {IsNotNull, "!= null"},
            {IsEmpty, "= String.Empty"},
            {IsNotEmpty, "!= String.Empty"}
        };

        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        public string Logic { get; set; }
        public List<FilterExpression> Filters { get; set; }

        public string ToExpression<TSource>(List<object> param)
        {
            if (!string.IsNullOrWhiteSpace(Logic) && Filters != null && Filters.Any())
            {
                var childFilters = Filters.Select(f => f.ToExpression<TSource>(param)).Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
                if (!childFilters.Any())
                    return null;
                return $"({childFilters.Aggregate((c, n) => string.Concat(c, " ", Logic, " ", n))})";
            }
                
            var fieldProperty = typeof(TSource).GetProperty(Field.ToUpperCamelCase());

            if (fieldProperty == null || string.IsNullOrWhiteSpace(Operator) || !Operators.ContainsKey(Operator))
                return null;

            string expression;

            if (Operator == IsNull || Operator == IsNotNull)
            {
                // Check first that the type can be set to null
                if (fieldProperty.PropertyType.IsValueType && Nullable.GetUnderlyingType(fieldProperty.PropertyType) == null)
                    return null;

                expression = $"{fieldProperty.Name} {Operators[Operator]}";
                return expression;
            }

            if (Operator == IsEmpty || Operator == IsNotEmpty)
            {
                if (fieldProperty.PropertyType != typeof(string))
                    return null;
                
                expression = $"{fieldProperty.Name} {Operators[Operator]}";
                return expression;
            }
            
            if (string.IsNullOrWhiteSpace(Value))
                return null;

            if (Operator == StartsWith || Operator == EndsWith || Operator == Contains || Operator == DoesNotContain)
            {
                if (string.IsNullOrWhiteSpace(Value))
                    return null;

                expression = $"{fieldProperty.Name}.{Operators[Operator]}(@{param.Count})";
                if (Operator == DoesNotContain)
                    expression = $"!{expression}";
                param.Add(Convert.ChangeType(Value, fieldProperty.PropertyType));
                return expression;
            }
            

            expression = $"{fieldProperty.Name} {Operators[Operator]} @{param.Count}";
            param.Add(Convert.ChangeType(Value, fieldProperty.PropertyType));
            return expression;
        }
        
    }
}