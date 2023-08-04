using Newtonsoft.Json;

namespace Api.ReferenceLib.Models
{
    public class LineAuthStateModel
    {
        [JsonProperty("state", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? State { get; set; }

        [JsonProperty("app_redirect_uri", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? AppRedirectUri { get; set; }

        [JsonProperty("line_redirect_uri", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? LineRedirectUri { get; set; }

        [JsonProperty("secret_key", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? SecretKey { get; set; }

        [JsonProperty("access_token", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? AccessToken { get; set; }

        [JsonProperty("group_user_id", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? GroupUserId { get; set; }

        [JsonProperty("code", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? Code { get; set; }

        [JsonProperty("line_access_token", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? LineAccessToken { get; set; }

        [JsonProperty("platform_id", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? PlatformId { get; set; }
    }
}