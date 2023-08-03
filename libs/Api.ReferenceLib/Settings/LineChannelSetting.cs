using Newtonsoft.Json;

namespace Api.ReferenceLib.Setttings
{
    public class LineChannelSetting
    {
        [JsonProperty("secret_id")]
        public virtual string? SecretId { get; set; }

        [JsonProperty("client_id")]
        public virtual string? ClientId { get; set; }

        [JsonProperty("channel_access_token")]
        public virtual string? ChannelAccessToken { get; set; }
    }
}