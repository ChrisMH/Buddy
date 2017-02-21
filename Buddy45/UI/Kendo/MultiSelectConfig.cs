using System.Collections.Generic;

namespace Buddy.UI.Kendo
{
    public class MultiSelectValue
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class MultiSelectConfig
    {
        public List<string> Defaults { get; set; }
        public List<MultiSelectValue> Values { get; set;}
    }
    
}
