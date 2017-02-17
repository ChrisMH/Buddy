using System.Collections.Generic;
using System.Linq;
using Buddy.Utility;

namespace Buddy.Web.TabularQuery
{
    public class FilterExpression
    {
        protected readonly IDictionary<string, string> Operators = new Dictionary<string, string>
        {
            {"eq", "="},
            {"neq", "!="},
            {"lt", "<"},
            {"lte", "<="},
            {"gt", ">"},
            {"gte", ">="},
            {"startswith", "StartsWith"},
            {"endswith", "EndsWith"},
            {"contains", "Contains"},
            {"doesnotcontain", "Contains"}
        };

        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        public string Logic { get; set; }
        public List<FilterExpression> Filters { get; set; }

        public string ToExpression()
        {
            if (!string.IsNullOrWhiteSpace(Logic) && Filters != null && Filters.Any())
                return $"({Filters.Select(f => f.ToExpression()).Where(e => !string.IsNullOrWhiteSpace(e)).Aggregate((c, n) => string.Concat(c, " ", Logic, " ", n))})";

            if (string.IsNullOrWhiteSpace(Field) || string.IsNullOrWhiteSpace(Operator) | !Operators.ContainsKey(Operator))
                return null;

            return $"{Field.ToUpperCamelCase()} {Operators[Operator]} {Value}";
        }
    }
}