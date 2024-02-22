using Api.LmcLib.DTOs;
using Api.LmcLib.Interfaces;
using Api.LmcLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.AuthSv.Controllers
{
    [ApiController]
    [Route("api/auth/v1")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authRepo;
        public AuthController(IAuthService authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost]
        [Route("line/state")]
        public async Task<ActionResult<Response>> CreateLineAuthState(AuthDto authDto)
        {
            return StatusCode(StatusCodes.Status201Created, await _authRepo.CreateLineAuthState(authDto));
        }

        // [HttpGet]
        // [Route("line/states")]
        // public ActionResult<Response> GetLineAuthStates()
        // {
        //     return StatusCode(StatusCodes.Status200OK, _authRepo.GetLineAuthStates());
        // }

        // [HttpPut]
        // [Route("line/state/{state}")]
        // public async Task<ActionResult<Response>> UpdateLineAuthState([FromRoute] string state, [FromBody] AuthDto authDto)
        // {
        //     return StatusCode(StatusCodes.Status200OK, await _authRepo.UpdateLineAuthState(state, authDto));
        // }
    }
}