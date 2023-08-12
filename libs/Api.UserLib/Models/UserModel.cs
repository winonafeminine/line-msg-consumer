using Api.ReferenceLib.DTOs;
using Api.ReferenceLib.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.UserLib.Models
{
    public class UserModel : BaseEntity
    {
        public UserModel()
        {
            UserId=ObjectId.GenerateNewId().ToString();
        }
        [JsonProperty("_id")]
        [BsonElement("_id")]
        public virtual string? UserId { get; set; }
        
        [JsonProperty("client_id")]
        [BsonElement("client_id")]
        public virtual string? ClientId { get; set; }

        [JsonProperty("group_user_id")]
        [BsonElement("group_user_id")]
        public virtual string? GroupUserId { get; set; }

        [JsonProperty("line_user_id")]
        [BsonElement("line_user_id")]
        public virtual string? LineUserId { get; set; }

        [JsonProperty("username")]
        [BsonElement("username")]
        public virtual string? Username { get; set; }

        [JsonProperty("display_name")]
        [BsonElement("display_name")]
        public virtual string? DisplayName { get; set; }

        [JsonProperty("email")]
        [BsonElement("email")]
        public virtual string? Email { get; set; }

        [JsonProperty("picture_url")]
        [BsonElement("picture_url")]
        public virtual string? PictureUrl { get; set; }

        [JsonProperty("status_message")]
        [BsonElement("status_message")]
        public virtual string? StatusMessage { get; set; }

        [JsonProperty("token")]
        [BsonElement("token")]
        public virtual LineLoginIssueTokenResponseDto? Token { get; set; }

        [JsonProperty("platform")]
        [BsonElement("platform")]
        public virtual UserPlatformModel? Platform { get; set; }
    }

    public class UserPlatformModel{

        [JsonProperty("platform_id")]
        [BsonElement("platform_id")]
        public string? PlatformId { get; set; }

        [JsonProperty("is_verified")]
        [BsonElement("is_verified")]
        public bool IsVerified { get; set; }
    }

}