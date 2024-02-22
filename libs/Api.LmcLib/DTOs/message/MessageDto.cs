using Api.LmcLib.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.LmcLib.DTOs
{
    public class MessageDto : MessageModel
    {
        [JsonProperty("message_id", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("message_id")]
        public override string? MessageId { get; set; }

        [JsonProperty("client_id", NullValueHandling=NullValueHandling.Ignore)]
        public override string? ClientId { get; set; }

        [JsonProperty("group_id", NullValueHandling=NullValueHandling.Ignore)]
        public override string? GroupId { get; set; }

        [JsonProperty("group_user_id", NullValueHandling=NullValueHandling.Ignore)]
        public override string? GroupUserId { get; set; }

        [JsonProperty("line_user_id", NullValueHandling=NullValueHandling.Ignore)]
        public override string? LineUserId { get; set; }

        [JsonProperty("message_type", NullValueHandling=NullValueHandling.Ignore)]
        public override string? MessageType { get; set; }

        [JsonProperty("from", NullValueHandling=NullValueHandling.Ignore)]
        public override MessageFromModel? From { get; set; }

        [JsonProperty("to", NullValueHandling=NullValueHandling.Ignore)]
        public override MessageToModel? To { get; set; }

        [JsonProperty("message_object", NullValueHandling=NullValueHandling.Ignore)]
        public override object? MessageObject { get; set; }

        [JsonProperty("platform_id", NullValueHandling=NullValueHandling.Ignore)]
        public override string? PlatformId { get; set; }

        [JsonProperty("user", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("user")]
        public virtual MessageUserDto? User { get; set; }
    }

    public class MessageUserDto
    {
        [JsonProperty("group_user_id")]
        [BsonElement("group_user_id")]
        public virtual string? GroupUserId { get; set; }

        [JsonProperty("display_name")]
        [BsonElement("display_name")]
        public virtual string? DisplayName { get; set; }

        [JsonProperty("picture_url")]
        [BsonElement("picture_url")]
        public virtual string? PictureUrl { get; set; }
    }
}