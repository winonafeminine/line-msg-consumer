using Newtonsoft.Json;

namespace Api.CommonLib.DTOs
{
    public class CommonRpcRequest
    {
        [JsonProperty("action")]
        public string? Action { get; set; }

        [JsonProperty("body")]
        public object? Body { get; set; }
    }
}