using Api.PlatformLib.Models;
using Newtonsoft.Json;

namespace Api.PlatformLib.DTOs
{
    public class PlatformDto : PlatformModel
    {
        [JsonProperty("platform_id", NullValueHandling=NullValueHandling.Ignore)]
        public override string? PlatformId { get; set; }

        [JsonProperty("group_user_id", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? GroupUserId { get; set; }
    }
}