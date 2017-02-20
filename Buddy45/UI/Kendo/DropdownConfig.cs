using System.Collections.Generic;

namespace Buddy.UI.Kendo
{
    public class DropdownValue<TId>
    {
        public TId Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownConfig<TId, TValue>
        where TValue : DropdownValue<TId>
    {
        public TId Default { get; set; }
        public List<TValue> Values { get; set;}
    }
}
