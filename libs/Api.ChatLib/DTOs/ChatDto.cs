using Api.CommonLib.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.ChatLib.DTOs
{
    public class ChatDto : BaseEntity
    {
        [JsonProperty("group")]
        [BsonElement("group")]
        public virtual ChatGroupDto? Group { get; set; }

        [JsonProperty("latest_message")]
        [BsonElement("latest_message")]
        public virtual ChatLatestMessageDto? LatestMessage { get; set; }
    }

    public class ChatGroupDto
    {
        [JsonProperty("group_id")]
        [BsonElement("group_id")]
        public virtual string? GroupId { get; set; }

        [JsonProperty("group_name")]
        [BsonElement("group_name")]
        public virtual string? GroupName { get; set; }
    }

    public class ChatLatestMessageDto
    {
        [JsonProperty("message_id")]
        [BsonElement("message_id")]
        public virtual string? MessageId { get; set; }

        [JsonProperty("text")]
        [BsonElement("text")]
        public virtual string? Text { get; set; }

        [JsonProperty("group_user_id")]
        [BsonElement("group_user_id")]
        public virtual string? GroupUserId { get; set; }

        [JsonProperty("latest_date")]
        [BsonElement("latest_date")]
        public virtual DateTime? LatestDate { get; set; }
    }
}