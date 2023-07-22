using Api.CommonLib.Models;
using Newtonsoft.Json;

namespace Api.UserLib.DTOs
{
    public class UserDto : BaseEntity
    {
        [JsonProperty("client_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? ClientId { get; set; }

        [JsonProperty("group_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? GroupId { get; set; }

        [JsonProperty("line_user_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? LineUserId { get; set; }

        [JsonProperty("username", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? Username { get; set; }

        [JsonProperty("display_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? DisplayName { get; set; }

        [JsonProperty("email", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? Email { get; set; }

        [JsonProperty("latest_message", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual UserLatestMessageDto? LatestMessage { get; set; }

    }

    public class UserLatestMessageDto
    {
        [JsonProperty("message_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? MessageId { get; set; }

        [JsonProperty("is_read", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual bool IsRead { get; set; }
    }
}