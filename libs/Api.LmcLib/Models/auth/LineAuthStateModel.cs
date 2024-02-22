using Api.LmcLib.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.LmcLib.Models
{
    public class LineAuthStateModel
    {
        public LineAuthStateModel()
        {
            AuthId=ObjectId.GenerateNewId().ToString();
        }
        [JsonProperty("_id", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("_id")]
        public virtual string? AuthId { get; set; }

        [JsonProperty("state", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("state")]
        public virtual string? State { get; set; }

        [JsonProperty("app_redirect_uri", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("app_redirect_uri")]
        public virtual string? AppRedirectUri { get; set; }

        [JsonProperty("line_redirect_uri", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("line_redirect_uri")]
        public virtual string? LineRedirectUri { get; set; }

        [JsonProperty("secret_key", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("secret_key")]
        public virtual string? SecretKey { get; set; }

        [JsonProperty("access_token", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("access_token")]
        public virtual string? AccessToken { get; set; }

        [JsonProperty("group_user_id", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("group_user_id")]
        public virtual string? GroupUserId { get; set; }

        [JsonProperty("display_name", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("display_name")]
        public virtual string? DisplayName { get; set; }

        [JsonProperty("picture_url", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("picture_url")]
        public virtual string? PictureUrl { get; set; }

        [JsonProperty("status_message", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("status_message")]
        public virtual string? StatusMessage { get; set; }

        [JsonProperty("code", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("code")]
        public virtual string? Code { get; set; }

        [JsonProperty("line_access_token", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("line_access_token")]
        public virtual string? LineAccessToken { get; set; }

        [JsonProperty("platform_id", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("platform_id")]
        public virtual string? PlatformId { get; set; }

        [JsonProperty("token", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("token")]
        public virtual LineLoginIssueTokenResponseDto? Token { get; set; }

        [JsonProperty("user_profile", NullValueHandling=NullValueHandling.Ignore)]
        [BsonElement("user_profile")]
        public virtual LineLoginUserProfileResponseDto? UserProfile { get; set; }
    }
}