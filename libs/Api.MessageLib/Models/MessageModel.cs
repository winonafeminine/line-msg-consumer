using Api.MessageLib.DTOs;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Api.MessageLib.Models
{
    public class MessageModel : MessageDto
    {
        public MessageModel()
        {
            MessageId=ObjectId.GenerateNewId().ToString();
        }
        [JsonProperty("_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? MessageId { get; set; }
    }
}