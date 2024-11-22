using Microsoft.AspNetCore.Mvc;
using Postie.Dtos;
using Postie.Interfaces;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authenticationService)
        {
            _authService = authenticationService;
        }

        [HttpPost("Login")]
        public ActionResult<string> Login([FromBody] LoginRequest request)
        {
            var result = _authService.Login(request.Email, request.Password);
            if (string.IsNullOrEmpty(result))
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}
