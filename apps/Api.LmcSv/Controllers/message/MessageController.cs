using Api.LmcLib.Interfaces;
using Api.LmcLib.Parameters;
using Api.LmcLib.Routes;
using Api.LmcLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.LmcSv.Controllers
{
    [ApiController]
    [Route("api/message/v1")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageSv;
        private readonly IAuthValidateJwtToken _authJwtToken;
        public MessagesController(IAuthValidateJwtToken authJwtToken, IMessageService messageSv)
        {
            _authJwtToken = authJwtToken;
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
        public async Task<ActionResult<Response>> GetPlatformGroupMessages([FromRoute] MessageRoute route, [FromQuery] MessageParam param)
        {
            // The port number must match the port of the gRPC server.
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;
            var authReponse = await _authJwtToken.ValidateJwtToken(token);
            param.PlatformId=authReponse.PlatformId;

            var messages = _messageSv.GetMessages(route, param);
            return Ok(new Response{
                Data=messages,
                StatusCode=StatusCodes.Status200OK
            });
        }
    }
}