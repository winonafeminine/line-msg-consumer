using Api.AuthLib.Interfaces;
using Api.MessageLib.Interfaces;
using Api.MessageLib.Models;
using Api.MessageLib.Parameters;
using Api.MessageLib.Routes;
using Microsoft.AspNetCore.Mvc;

namespace Api.MessageSv.Controllers
{
    [ApiController]
    [Route("/message/v1")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageSv;
        private readonly IAuthGrpcClientService _authGrpcClient;
        public MessagesController(IAuthGrpcClientService authGrpcClient, IMessageService messageSv)
        {
            _authGrpcClient = authGrpcClient;
            _messageSv = messageSv;
        }

        [HttpPost]
        [Route("webhook/line/{id}")]
        public async Task<ActionResult> RetrieveLineMessage([FromBody] object content, string id)
        {
            await Task.Yield();
            string signature = HttpContext.Request.Headers["X-Line-Signature"].ToString();
            await _messageSv.RetriveLineMessage(content, signature, id);
            return Ok();
        }

        [HttpGet]
        [Route("group/{group_id}/messages")]
        public async Task<ActionResult<IEnumerable<MessageModel>>> GetPlatformGroupMessages([FromRoute] MessageRoute route, [FromQuery] MessageParam param)
        {
            // The port number must match the port of the gRPC server.
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
            var authReponse = await _authGrpcClient.ValidateJwtToken(token);
            param.PlatformId=authReponse.PlatformId;

            var messages = _messageSv.GetMessages(route, param);
            return Ok(messages);
        }
    }
}