using Api.UserLib.DTOs;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Api.UserLib.Models
{
    public class UserModel : UserDto
    {
        public UserModel()
        {
            UserId=ObjectId.GenerateNewId().ToString();
        }
        [JsonProperty("_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? UserId { get; set; }
    }
}