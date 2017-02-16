using System.Collections.Generic;
using Newtonsoft.Json;

namespace Buddy.Web.TabularQuery
{
    public class TabularResponse<TRow>
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        
        [JsonProperty(PropertyName = "data")]
        public List<TRow> Data { get; set; }
        
        [JsonProperty(PropertyName = "aggregates")]
        public object Aggregates { get; set; }
    }
}
