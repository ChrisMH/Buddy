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
            {StartsWith, ""},
            {EndsWith, ""},
            {Contains, ""},
            {DoesNotContain, ""},
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
            
            if (Operator == IsNull || Operator == IsNotNull)
            {
                // Check first that the type can be set to null
                if (fieldProperty.PropertyType.IsValueType && Nullable.GetUnderlyingType(fieldProperty.PropertyType) == null)
                    return null;

                if(Operator == IsNull)
                {
                    return $"{fieldProperty.Name} == null";
                }
                return $"{fieldProperty.Name} != null";
            }

            if (Operator == IsEmpty || Operator == IsNotEmpty)
            {
                if (fieldProperty.PropertyType != typeof(string))
                    return null;

                param.Add(String.Empty);
                if (Operator == IsEmpty)
                {
                    return $"{fieldProperty.Name} == @{param.Count - 1}";
                }
                return $"{fieldProperty.Name} != @{param.Count - 1}";
            }
            
            if (string.IsNullOrWhiteSpace(Value))
                return null;

            if (Operator == StartsWith || Operator == EndsWith || Operator == Contains || Operator == DoesNotContain)
            {
                if (fieldProperty.PropertyType != typeof(string))
                    return null;

                param.Add(Convert.ChangeType(Value, fieldProperty.PropertyType));
                param.Add(StringComparison.OrdinalIgnoreCase);

                if (Operator == StartsWith) 
                { 
                    return $"{fieldProperty.Name}.StartsWith(@{param.Count - 2}, @{param.Count - 1})";
                }
                else if (Operator == EndsWith)
                {
                    return $"{fieldProperty.Name}.EndsWith(@{param.Count - 2}, @{param.Count - 1})";
                }
                else if (Operator == Contains)
                {
                    return $"{fieldProperty.Name}.IndexOf(@{param.Count - 2}, @{param.Count - 1}) >= 0";
                }
                return $"{fieldProperty.Name}.IndexOf(@{param.Count - 2}, @{param.Count - 1}) < 0";
            }
            
            if (fieldProperty.PropertyType == typeof(string) && (Operator == Eq || Operator == Neq))
            {
                param.Add(Convert.ChangeType(Value, fieldProperty.PropertyType));
                param.Add(StringComparison.OrdinalIgnoreCase);

                if(Operator == Eq)
                {
                    return $"{fieldProperty.Name}.Equals(@{param.Count - 2}, @{param.Count - 1})";
                }
                return $"!{fieldProperty.Name}.Equals(@{param.Count - 2}, @{param.Count - 1})";
            }
            
            param.Add(Convert.ChangeType(Value, fieldProperty.PropertyType));
            return $"{fieldProperty.Name} {Operators[Operator]} @{param.Count - 1}";
        }
        
    }
}