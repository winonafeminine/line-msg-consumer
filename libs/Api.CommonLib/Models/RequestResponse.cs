using Api.CommonLib.Exceptions;
using Newtonsoft.Json;

namespace Api.CommonLib.Models
{
    public class RequestResponse
    {
        public RequestResponse()
        {
            Data=new List<Error>();
        }

        [JsonProperty("data", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual object Data { get; set; }

        [JsonProperty("status_code", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual int StatusCode { get; set; }
    }
}