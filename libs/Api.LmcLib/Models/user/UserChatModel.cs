using Api.LmcLib.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.LmcLib.Models
{
    public class UserChatModel : BaseEntity
    {
        public UserChatModel()
        {
            UserChatId=ObjectId.GenerateNewId().ToString();
        }
        [JsonProperty("_id")]
        [BsonElement("_id")]
        public virtual string? UserChatId { get; set; }
        
        [JsonProperty("chat_id")]
        [BsonElement("chat_id")]
        public virtual string? ChatId { get; set; }

        [JsonProperty("group_id")]
        [BsonElement("group_id")]
        public virtual string? GroupId { get; set; }

        [JsonProperty("user_id")]
        [BsonElement("user_id")]
        public virtual string? UserId { get; set; }

        [JsonProperty("group_user_id")]
        [BsonElement("group_user_id")]
        public virtual string? GroupUserId { get; set; }

        [JsonProperty("line_user_id")]
        [BsonElement("line_user_id")]
        public virtual string? LineUserId { get; set; }

        [JsonProperty("platform_id")]
        [BsonElement("platform_id")]
        public virtual string? PlatformId { get; set; }

        [JsonProperty("latest_message")]
        [BsonElement("latest_message")]
        public virtual UserChatLatestMessageModel? LatestMessage { get; set; }
    }

    public class UserChatLatestMessageModel{
        [JsonProperty("message_id")]
        [BsonElement("message_id")]
        public virtual string? MessageId { get; set; }

        [JsonProperty("is_read")]
        [BsonElement("is_read")]
        public virtual bool IsRead { get; set; }
    }
}