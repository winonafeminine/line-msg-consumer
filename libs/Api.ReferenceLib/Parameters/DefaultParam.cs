using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.ReferenceLib.Parameters
{
    public class DefaultParam
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

        [FromQuery(Name = "limit")]
        [JsonProperty("limit")]
        public virtual int Limit { get; set; } = 10;

        [FromQuery(Name = "sort_by")]
        [JsonProperty("sort_by")]
        public virtual string? SortBy { get; set; }
        public Tuple<string, string> GetSortData()
        {
            var sorts = SortBy!.Split("-");
            return new Tuple<string, string>(sorts[0], sorts[1]);
        }
        public string GetSortField()
        {
            return GetSortData().Item1;
        }
        public int GetSortValue()
        {
            var sortNum = new Dictionary<string, int> {
                { "desc", -1 },
                { "asc", 1 }
            };
            return sortNum[GetSortData().Item2];
        }
    }
}