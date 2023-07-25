using Api.CommonLib.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.MessageLib.DTOs
{
    public class MessageDto : BaseEntity
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

        [JsonProperty("message_type")]
        [BsonElement("message_type")]
        public virtual string? MessageType { get; set; }

        [JsonProperty("from")]
        [BsonElement("from")]
        public virtual MessageFromDto? From { get; set; }

        [JsonProperty("to")]
        [BsonElement("to")]
        public virtual MessageToDto? To { get; set; }

        [JsonProperty("message_object")]
        [BsonElement("message_object")]
        public virtual object? MessageObject { get; set; }
    }

    public class MessageFromDto
    {
        [JsonProperty("from_id")]
        [BsonElement("from_id")]
        public virtual string? FromId { get; set; }

        [JsonProperty("from_name")]
        [BsonElement("from_name")]
        public virtual string? FromName { get; set; }
    }
    public class MessageToDto
    {
        [JsonProperty("to_id")]
        [BsonElement("to_id")]
        public virtual string? ToId { get; set; }

        [JsonProperty("to_name")]
        [BsonElement("to_name")]
        public virtual string? ToName { get; set; }
    }
}