using Api.LmcLib.Interfaces;
using Api.LmcLib.DTOs;
using Api.LmcLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.LmcSv.Controllers
{
    [Route("api/platform/v1")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformService _platformSv;
        private readonly IAuthValidateJwtToken _authJwtToken;
        public PlatformController(IPlatformRepository platformRepo, IPlatformService platformSv, IAuthValidateJwtToken authJwtToken)
        {
            _platformSv = platformSv;
            _authJwtToken = authJwtToken;
        }

        [HttpPut]
        [Route("verify")]
        public async Task<ActionResult<Response>> UpdatePlatform([FromRoute] string action, [FromBody] PlatformDto platformDto)
        {
            string? token = Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last()!;
            var authResponse = await _authJwtToken.ValidateJwtToken(token);
            return Ok(await _platformSv.UpdatePlatform("verify", authResponse.PlatformId!, platformDto));
        }
    }
}