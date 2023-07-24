using Api.ChatLib.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.ChatLib.Models
{
    public class ChatModel : ChatDto
    {
        public ChatModel()
        {
            ChatId=ObjectId.GenerateNewId().ToString();
        }
        [JsonProperty("_id")]
        [BsonElement("_id")]
        public virtual string? ChatId { get; set; }
    }
}