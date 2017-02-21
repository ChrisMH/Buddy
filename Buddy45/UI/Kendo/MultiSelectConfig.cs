using System.Collections.Generic;

namespace Buddy.UI.Kendo
{
    public class MultiSelectValue<TId>
    {
        public TId Id { get; set; }
        public string Name { get; set; }
    }

    public class MultiSelectConfig<TId>
    {
        public List<TId> Defaults { get; set; }
        public List<MultiSelectValue<TId>> Values { get; set;}
    }

    public class MultiSelectConfig<TId, TValue>
        where TValue : MultiSelectValue<TId>
    {
        public List<TId> Defaults { get; set; }
        public List<TValue> Values { get; set;}
    }
}
