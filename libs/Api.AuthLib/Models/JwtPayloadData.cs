using Newtonsoft.Json;

namespace Api.AuthLib.Models
{
    public class JwtPayloadData
    {
        [JsonProperty("platform_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? PlatformId { get; set; }

        [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? UserId { get; set; }

        [JsonProperty("nbf", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? NotValidBefore { get; set; }

        [JsonProperty("exp", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? Expires { get; set; }

        [JsonProperty("iat", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? IssuedAt { get; set; }

        [JsonProperty("iss", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? Issuer { get; set; }
    }
}