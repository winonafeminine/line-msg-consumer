using Microsoft.AspNetCore.Mvc;

namespace Api.MessageLib.Routes
{
    public class MessageRoute
    {
        [FromRoute(Name = "platform_id")]
        public string? PlatformId { get; set; }

        [FromRoute(Name = "group_id")]
        public string? GroupId { get; set; }
    }
}