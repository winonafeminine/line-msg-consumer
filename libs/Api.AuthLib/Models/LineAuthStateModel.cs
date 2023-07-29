using Newtonsoft.Json;

namespace Api.AuthLib.Models
{
    public class LineAuthStateModel
    {
        [JsonProperty("state", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? State { get; set; }

        [JsonProperty("app_redirect_uri", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? AppRedirectUri { get; set; }

        [JsonProperty("line_redirect_uri", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? LineRedirectUri { get; set; }

        [JsonProperty("secret_id", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? SecretId { get; set; }

        [JsonProperty("access_token", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? AccessToken { get; set; }

        [JsonProperty("line_group_id", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? LineGroupId { get; set; }
    }
}