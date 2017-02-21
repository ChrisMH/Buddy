using System.Collections.Generic;

namespace Buddy.UI.Kendo
{
    public class DropDownValue
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropDownConfig
    {
        public string Default { get; set; }
        public List<DropDownValue> Values { get; set;}
    }
    
}
