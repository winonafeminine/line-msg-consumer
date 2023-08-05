using Api.ReferenceLib.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.PlatformLib.Models
{
    public class PlatformModel : BaseEntity
    {
        [JsonProperty("_id")]
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public virtual string? PlatformId { get; set; }

        [JsonProperty("platform_name")]
        [BsonElement("platform_name")]
        public virtual string? PlatformName { get; set; }

        [JsonProperty("access_token")]
        [BsonElement("access_token")]
        public virtual string? AccessToken { get; set; }

        [JsonProperty("secret_key")]
        [BsonElement("secret_key")]
        public virtual string? SecretKey { get; set; }

        [JsonProperty("is_verified")]
        [BsonElement("is_verified")]
        public virtual bool IsVerified { get; set; }
    }
}