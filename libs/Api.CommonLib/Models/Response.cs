using Newtonsoft.Json;

namespace Api.CommonLib.Models
{
    public class Response
    {
        [JsonProperty("data", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual object? Data { get; set; }

        [JsonProperty("status_code", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual int StatusCode { get; set; }

        [JsonProperty("message", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? Message { get; set; }
    }
}