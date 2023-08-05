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
        public PlatformController(IPlatformRepository platformRepo, IPlatformService platformSv)
        {
            _platformSv = platformSv;
        }

        [HttpPut]
        [Route("verify/{platformId}")]
        public async Task<ActionResult<Response>> UpdatePlatform([FromRoute] string action, [FromRoute] string platformId, [FromBody] PlatformDto platformDto)
        {
            return Ok(await _platformSv.UpdatePlatform("verify", platformId, platformDto));
        }
    }
}