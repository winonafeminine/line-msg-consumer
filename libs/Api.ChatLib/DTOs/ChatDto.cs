using Api.CommonLib.Models;
using Newtonsoft.Json;

namespace Api.ChatLib.DTOs
{
    public class ChatDto : BaseEntity
    {
        [JsonProperty("group", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual ChatGroupDto? Group { get; set; }

        [JsonProperty("latest_message", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual ChatLatestMessageDto? LatestMessage { get; set; }
    }

    public class ChatGroupDto
    {
        [JsonProperty("group_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? GroupId { get; set; }

        [JsonProperty("group_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? GroupName { get; set; }
    }

    public class ChatLatestMessageDto
    {
        [JsonProperty("message_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? MessageId { get; set; }

        [JsonProperty("text", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? Text { get; set; }

        [JsonProperty("line_user_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? LineUserId { get; set; }

        [JsonProperty("latest_date", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual DateTime? LatestDate { get; set; }
    }
}