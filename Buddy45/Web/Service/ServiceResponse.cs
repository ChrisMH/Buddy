using Newtonsoft.Json;

namespace Buddy.Web.Service
{
    public class ServiceResponse<TReturn>
    {
        public ServiceResponse()
        {
        }

        public ServiceResponse(bool success, TReturn data, string message = null)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "data")]
        public TReturn Data { get; set; }
    }
}
