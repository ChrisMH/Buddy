
using Buddy.Utility;

namespace Buddy.Web.TabularQuery
{
    /// <summary>
    /// Sorting part of a tabular query
    /// The query string part looks like: sort[0].field=field0&sort[0].dir=asc&sort[1].field=field1&sort[1].dir=desc ...
    /// Supported dir values:
    ///    asc
    ///    desc
    /// </summary>
    public class SortExpression
    {
        public string Field { get; set; }
        public string Dir { get; set; }

        public string ToDynamicLinqExpression()
        {
            return string.Concat(Field.ToUpperCamelCase(), " ", Dir);
        }
    }
}
