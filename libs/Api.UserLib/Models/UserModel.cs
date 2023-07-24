using Api.UserLib.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.UserLib.Models
{
    public class UserModel : UserDto
    {
        public UserModel()
        {
            UserId=ObjectId.GenerateNewId().ToString();
        }
        [JsonProperty("_id")]
        [BsonElement("_id")]
        public virtual string? UserId { get; set; }
    }
}