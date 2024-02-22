using Newtonsoft.Json;

namespace Api.LmcLib.DTOs
{
    public class AuthDto
    {
        [JsonProperty("app_redirect_uri", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? AppRedirectUri { get; set; }

        [JsonProperty("line_redirect_uri", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? LineRedirectUri { get; set; }

        [JsonProperty("code", NullValueHandling=NullValueHandling.Ignore)]
        public virtual string? Code { get; set; }
    }
}