using Api.CommonLib.Models;
using Newtonsoft.Json;

namespace Api.MessageLib.DTOs
{
    public class MessageDto : BaseEntity
    {
        [JsonProperty("client_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? ClientId { get; set; }

        [JsonProperty("group_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? GroupId { get; set; }

        [JsonProperty("line_user_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? LineUserId { get; set; }

        [JsonProperty("message_type", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? MessageType { get; set; }

        [JsonProperty("from", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual MessageFromDto? From { get; set; }

        [JsonProperty("to", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual MessageToDto? To { get; set; }

        [JsonProperty("message_object", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual object? MessageObject { get; set; }
    }

    public class MessageFromDto
    {
        [JsonProperty("from_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? FromId { get; set; }

        [JsonProperty("from_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? FromName { get; set; }
    }
    public class MessageToDto
    {
        [JsonProperty("to_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? ToId { get; set; }

        [JsonProperty("to_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? ToName { get; set; }
    }
}