using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.LmcLib.DTOs
{
    public class LineLoginDto
    {
        
    }
    
    // {
    //     "access_token": "bNl4YEFPI/hjFWhTqexp4MuEw5YPs...",
    //     "expires_in": 2592000,
    //     "id_token": "eyJhbGciOiJIUzI1NiJ9...",
    //     "refresh_token": "Aa1FdeggRhTnPNNpxr8p",
    //     "scope": "profile",
    //     "token_type": "Bearer"
    // }
    public class LineLoginIssueTokenResponseDto
    {
        [JsonProperty("access_token")]
        [BsonElement("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("expires_in")]
        [BsonElement("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("id_token")]
        [BsonElement("id_token")]
        public string? IdToken { get; set; }

        [JsonProperty("refresh_token")]
        [BsonElement("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonProperty("scope")]
        [BsonElement("scope")]
        public string? Scope { get; set; }

        [JsonProperty("token_type")]
        [BsonElement("token_type")]
        public string? TokenType { get; set; }
    }

    public class LineLoginUserProfileResponseDto
    {
        [JsonProperty("userId")]
        public string? UserId { get; set; }

        [JsonProperty("displayName")]
        public string? DisplayName { get; set; }

        [JsonProperty("pictureUrl")]
        public string? PictureUrl { get; set; }

        [JsonProperty("statusMessage")]
        public string? StatusMessage { get; set; }

    }
}