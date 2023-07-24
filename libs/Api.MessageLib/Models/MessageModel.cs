using Api.MessageLib.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.MessageLib.Models
{
    public class MessageModel : MessageDto
    {
        public MessageModel()
        {
            MessageId=ObjectId.GenerateNewId().ToString();
        }
        [JsonProperty("_id")]
        [BsonElement("_id")]
        public virtual string? MessageId { get; set; }
    }
}