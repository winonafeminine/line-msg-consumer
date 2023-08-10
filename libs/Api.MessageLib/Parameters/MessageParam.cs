
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.MessageLib.Parameters
{
    public class MessageParam
    {
        [FromQuery(Name = "platform_id")]
        [JsonProperty("platform_id")]
        public virtual string? PlatformId { get; set; }

        [FromQuery(Name = "group_id")]
        [JsonProperty("group_id")]
        public virtual string? GroupId { get; set; }

        [FromQuery(Name = "group_user_id")]
        [JsonProperty("group_user_id")]
        public virtual string? GroupUserId { get; set; }

        [FromQuery(Name = "types")]
        [JsonProperty("types")]
        public virtual string? Types { get; set; }

        [FromQuery(Name = "limit")]
        [JsonProperty("limit")]
        public virtual int Limit { get; set; } = 10;

        [FromQuery(Name = "sort_by")]
        [JsonProperty("sort_by")]
        public virtual string? SortBy { get; set; }

        [FromQuery(Name = "start_date")]
        [JsonProperty("start_date")]
        public virtual string? StartDate { get; set; }

        [FromQuery(Name = "text")]
        [JsonProperty("text")]
        public virtual string? Text { get; set; }

        public List<string> GetMessageTypes()
        {
            return Types!.Split(",").ToList();
        }
        public Tuple<string, string> GetSortData()
        {
            var sorts = SortBy!.Split("-");
            return new Tuple<string, string>(sorts[0], sorts[0]);
        }
    }
}