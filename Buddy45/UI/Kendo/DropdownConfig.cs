using System.Collections.Generic;

namespace Buddy.UI.Kendo
{
    public class DropDownValue<TId>
    {
        public TId Id { get; set; }
        public string Name { get; set; }
    }

    public class DropDownConfig<TId>
    {
        public TId Default { get; set; }
        public List<DropDownValue<TId>> Values { get; set;}
    }

    public class DropDownConfig<TId, TValue>
        where TValue : DropDownValue<TId>
    {
        public TId Default { get; set; }
        public List<TValue> Values { get; set;}
    }
}
