using Microsoft.AspNetCore.Mvc;
using Sales.API.Interfaces.Services;
using Sales.API.Models.DTOs;
using Sales.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Sales.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>JWT token if authentication is successful</returns>
        /// <response code="200">Returns the JWT token</response>
        /// <response code="401">Invalid username or password</response>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "User login", Description = "Authenticates a user and returns a JWT token.")]
        [SwaggerResponse(200, "Successful login", typeof(object))]
        [SwaggerResponse(401, "Unauthorized - Invalid credentials", typeof(ApiErrorResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var token = await _authService.AuthenticateAsync(loginRequest);

            if (string.IsNullOrEmpty(token))
                return Unauthorized(new ApiErrorResponse("AuthenticationError", "Cannot login", "Invalid username or password"));

            return Ok(new { token });
        }
    }
}
