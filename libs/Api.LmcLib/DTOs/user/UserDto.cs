using Api.LmcLib.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.LmcLib.DTOs
{
    public class UserDto : BaseEntity
    {
        [JsonProperty("client_id")]
        [BsonElement("client_id")]
        public virtual string? ClientId { get; set; }

        [JsonProperty("group_id")]
        [BsonElement("group_id")]
        public virtual string? GroupId { get; set; }

        [JsonProperty("group_user_id")]
        [BsonElement("group_user_id")]
        public virtual string? GroupUserId { get; set; }

        [JsonProperty("line_user_id")]
        [BsonElement("line_user_id")]
        public virtual string? LineUserId { get; set; }

        [JsonProperty("username")]
        [BsonElement("username")]
        public virtual string? Username { get; set; }

        [JsonProperty("display_name")]
        [BsonElement("display_name")]
        public virtual string? DisplayName { get; set; }

        [JsonProperty("email")]
        [BsonElement("email")]
        public virtual string? Email { get; set; }

        [JsonProperty("picture_url")]
        [BsonElement("picture_url")]
        public virtual string? PictureUrl { get; set; }

        [JsonProperty("latest_message")]
        [BsonElement("latest_message")]
        public virtual UserLatestMessageDto? LatestMessage { get; set; }

    }

    public class UserLatestMessageDto
    {
        [JsonProperty("message_id")]
        [BsonElement("message_id")]
        public virtual string? MessageId { get; set; }

        [JsonProperty("is_read")]
        [BsonElement("is_read")]
        public virtual bool IsRead { get; set; }
    }
}