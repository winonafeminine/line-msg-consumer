using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.LmcLib.Models
{
    public class ChatModel : BaseEntity
    {
        public ChatModel()
        {
            ChatId = ObjectId.GenerateNewId().ToString();
        }
        [JsonProperty("_id")]
        [BsonElement("_id")]
        public virtual string? ChatId { get; set; }

        [JsonProperty("group")]
        [BsonElement("group")]
        public virtual ChatGroupModel? Group { get; set; }

        [JsonProperty("latest_message")]
        [BsonElement("latest_message")]
        public virtual ChatLatestMessageModel? LatestMessage { get; set; }

        [JsonProperty("platform_id")]
        [BsonElement("platform_id")]
        public virtual string? PlatformId { get; set; }
    }

    public class ChatGroupModel
    {
        [JsonProperty("group_id")]
        [BsonElement("group_id")]
        public virtual string? GroupId { get; set; }

        [JsonProperty("group_name")]
        [BsonElement("group_name")]
        public virtual string? GroupName { get; set; }

        [JsonProperty("picture_url")]
        [BsonElement("picture_url")]
        public virtual string? PictureUrl { get; set; }
    }

    public class ChatLatestMessageModel
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