using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Helpers;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthController()
        {
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new InvalidOperationException("JWT_SECRET_KEY not configured.");
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "your-issuer";
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "your-audience";

            _tokenGenerator = new JwtTokenGenerator(secretKey, issuer, audience);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Simulate user validation (in a real-world scenario, check from a database)
            if (request.Username == "admin" && request.Password == "password")
            {
                var token = _tokenGenerator.GenerateToken(
                    userId: "1",
                    userName: "admin",
                    email: "admin@example.com",
                    role: "Admin"
                );

                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Invalid username or password" });
        }
    }
    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
