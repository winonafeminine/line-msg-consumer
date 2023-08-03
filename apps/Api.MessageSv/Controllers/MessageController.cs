using Api.CommonLib.Interfaces;
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
            string signature = HttpContext.Request.Headers["X-Line-Signature"].ToString();
            await _lineMessagin.RetriveLineMessage(content, signature, id);
            return Ok();
        }
    }
}