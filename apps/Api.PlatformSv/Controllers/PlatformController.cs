using Api.AuthLib.Interfaces;
using Api.PlatformLib.DTOs;
using Api.PlatformLib.Interfaces;
using Api.ReferenceLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.PlatformSv.Controllers
{
    [Route("platform/v1")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformService _platformSv;
        private readonly IAuthGrpcClientService _authGrpc;
        public PlatformController(IPlatformRepository platformRepo, IPlatformService platformSv, IAuthGrpcClientService authGrpc)
        {
            _platformSv = platformSv;
            _authGrpc = authGrpc;
        }

        [HttpPut]
        [Route("verify")]
        public async Task<ActionResult<Response>> UpdatePlatform([FromRoute] string action, [FromBody] PlatformDto platformDto)
        {
            string? token = Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last()!;
            var authResponse = await _authGrpc.ValidateJwtToken(token);
            return Ok(await _platformSv.UpdatePlatform("verify", authResponse.PlatformId!, platformDto));
        }
    }
}