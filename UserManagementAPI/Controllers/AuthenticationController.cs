using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Helpers;
using UserManagementAPI.Interfaces;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly IUserRepository _repository;

        public AuthController(IConfiguration configuration, IUserRepository repository)
        {
            var jwtSection = configuration.GetSection("JWT");
            var secretKey = jwtSection["SecretKey"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT settings are not properly configured.");
            }

            _tokenGenerator = new JwtTokenGenerator(secretKey, issuer, audience);
            _repository = repository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validar credenciais do usuário
            var user = _repository.GetAllUsers().FirstOrDefault(u => 
                u.Username == request.Username && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            // Gerar token JWT
            var token = _tokenGenerator.GenerateToken(
                userId: user.Id.ToString(),
                userName: user.Username,
                email: user.Email,
                role: user.Role,
                expiryInHours: 2
            );

            return Ok(new
            {
                Token = token,
                ExpiresIn = 7200 // 2 horas em segundos
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
