using Api.LmcLib.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.LmcLib.DTOs
{
    public class ChatDto : ChatModel
    {
        [JsonProperty("chat_id")]
        [BsonElement("chat_id")]
        public override string? ChatId { get; set; }
    }
}