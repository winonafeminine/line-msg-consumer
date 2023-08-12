using Api.AuthLib.Interfaces;
using Api.ChatLib.DTOs;
using Api.ChatLib.Interfaces;
using Api.ChatLib.Parameters;
using Api.ReferenceLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.ChatSv.Controllers
{
    [ApiController]
    [Route("chat/v1")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatSv;
        private readonly IAuthGrpcClientService _authGrpc;
        public ChatController(IChatService chatSv, IAuthGrpcClientService authGrpc)
        {
            _chatSv = chatSv;
            _authGrpc = authGrpc;
        }

        [HttpGet]
        [Route("groups")]
        public async Task<ActionResult<Response>> GetChats([FromQuery] ChatParam param)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!;;
            var authRes = await _authGrpc.ValidateJwtToken(token);
            param.PlatformId=authRes.PlatformId;
            return Ok(new Response{
                Data=_chatSv.GetChats(param),
                StatusCode=StatusCodes.Status200OK
            });
        }
    }
}