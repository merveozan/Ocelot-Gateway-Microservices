using IdentityServerApi.Models;
using IdentityServerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenHandler _jwtTokenHandler;

        public AuthController(JwtTokenHandler jwtTokenHandler)
        {
            _jwtTokenHandler = jwtTokenHandler;

        }

        [HttpPost]

        public ActionResult<AuthenticationResponse?> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
        {
            var authenticationResponse = _jwtTokenHandler.GenerateJwtToken(authenticationRequest);
            if (authenticationResponse == null) return Unauthorized();
            return authenticationResponse;

        }

    }
}
