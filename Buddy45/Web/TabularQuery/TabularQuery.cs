using System.Collections.Generic;

namespace Buddy.Web.TabularQuery
{
    public class TabularQuery
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        
        public List<AggregateExpression> Aggregate { get; set; }
        public FilterExpression Filter { get; set; }
        public List<SortExpression> Sort { get; set; }
        public List<GroupExpression> Group { get; set; }
    }
}