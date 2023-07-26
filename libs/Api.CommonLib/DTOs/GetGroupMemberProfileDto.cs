using Newtonsoft.Json;

namespace Api.CommonLib.DTOs
{
    public class GetGroupMemberProfileDto
    {
        [JsonProperty("displayName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? DisplayName { get; set; }

        [JsonProperty("userId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? UserId { get; set; }

        [JsonProperty("pictureUrl", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? PictureUrl { get; set; }
    }
}