using Api.LmcLib.Interfaces;
using Api.LmcLib.Parameters;
using Api.LmcLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.ChatSv.Controllers
{
    [ApiController]
    [Route("api/chat/v1")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatSv;
        private readonly IAuthValidateJwtToken _authJwtToken;
        public ChatController(IChatService chatSv, IAuthValidateJwtToken authJwtToken)
        {
            _chatSv = chatSv;
            _authJwtToken = authJwtToken;
        }

        [HttpGet]
        [Route("groups")]
        public async Task<ActionResult<Response>> GetChats([FromQuery] ChatParam param)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;;
            var authRes = await _authJwtToken.ValidateJwtToken(token);
            param.PlatformId=authRes.PlatformId;
            return Ok(new Response{
                Data=_chatSv.GetChats(param),
                StatusCode=StatusCodes.Status200OK
            });
        }
    }
}