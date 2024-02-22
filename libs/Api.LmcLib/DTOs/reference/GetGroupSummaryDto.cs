using Newtonsoft.Json;

namespace Api.LmcLib.DTOs
{
    public class GetGroupSummaryDto : GetGroupMemberProfileDto
    {
        [JsonProperty("groupId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? GroupId { get; set; }

        [JsonProperty("groupName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? GroupName { get; set; }
    }
}