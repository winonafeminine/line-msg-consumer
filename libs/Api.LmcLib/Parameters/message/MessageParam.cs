
using Api.LmcLib.Parameters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.LmcLib.Parameters
{
    public class MessageParam : DefaultParam
    {
        [FromQuery(Name = "types")]
        [JsonProperty("types")]
        public virtual string? Types { get; set; }

        [FromQuery(Name = "start_date")]
        [JsonProperty("start_date")]
        public virtual string? StartDate { get; set; }

        [FromQuery(Name = "text")]
        [JsonProperty("text")]
        public virtual string? Text { get; set; }

        public string[] GetMessageTypes()
        {
            return Types!.Split(",");
        }
    }
}