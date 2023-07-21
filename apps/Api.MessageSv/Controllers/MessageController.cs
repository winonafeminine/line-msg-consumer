using Api.MessageLib.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.MessageSv.Controllers
{
    [ApiController]
    [Route("/message/v1")]
    public class MessagesController : ControllerBase
    {
        private readonly ILineMessaging _lineMessagin;
        public MessagesController(ILineMessaging lineMessagin)
        {
            _lineMessagin = lineMessagin;
        }

        [HttpPost]
        [Route("webhook/line/{id}")]
        public async Task<ActionResult> RetrieveLineMessage([FromBody] object content, string id)
        {
            await Task.Yield();
            await _lineMessagin.RetriveLineMessage(content, id);
            return Ok();
        }
    }
}