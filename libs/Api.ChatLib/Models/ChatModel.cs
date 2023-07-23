using Api.ChatLib.DTOs;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Api.ChatLib.Models
{
    public class ChatModel : ChatDto
    {
        public ChatModel()
        {
            ChatId=ObjectId.GenerateNewId().ToString();
        }
        [JsonProperty("_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? ChatId { get; set; }
    }
}