using Newtonsoft.Json;

namespace Api.UserLib.DTOs
{
    public class GetGroupMemberProfileDto
    {
        [JsonProperty("displayName")]
        public virtual string? DisplayName { get; set; }

        [JsonProperty("userId")]
        public virtual string? UserId { get; set; }

        [JsonProperty("pictureUrl")]
        public virtual string? PictureUrl { get; set; }
    }
}