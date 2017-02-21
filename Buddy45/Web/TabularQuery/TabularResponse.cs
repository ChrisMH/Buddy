using System.Collections.Generic;
using Newtonsoft.Json;

namespace Buddy.Web.TabularQuery
{
    public class TabularResponse
    {
        /// <summary>
        /// Total number of items
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        
        /// <summary>
        /// Calculated aggregates
        /// </summary>
        [JsonProperty(PropertyName = "aggregates")]
        public object Aggregates { get; set; }
        
        /// <summary>
        /// List of items if not grouped, otherwise list of TabularGroup
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public object Items { get; set; }
    }

    public class TabularGroup
    {
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "aggregates")]
        public object Aggregates { get; set; }
        
        [JsonProperty(PropertyName = "hasSubgroups")]
        public bool HasSubGroups {get; set; }

        [JsonProperty(PropertyName = "items")]
        public object Items { get; set; }
    }
}
