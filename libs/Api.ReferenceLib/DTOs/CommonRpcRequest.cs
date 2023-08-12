using Newtonsoft.Json;

namespace Api.ReferenceLib.DTOs
{
    public class CommonRpcRequest
    {
        [JsonProperty("action")]
        public string? Action { get; set; }

        [JsonProperty("body")]
        public object? Body { get; set; }
    }
}