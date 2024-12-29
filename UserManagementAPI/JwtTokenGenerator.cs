using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserManagementAPI.Helpers
{
    /// <summary>
    /// Responsible for generating JWT tokens.
    /// </summary>
    public class JwtTokenGenerator
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        /// <summary>
        /// Constructor to initialize JWT configuration.
        /// </summary>
        public JwtTokenGenerator(string secretKey, string issuer, string audience)
        {
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException(nameof(secretKey), "Secret key cannot be null or empty.");
            if (string.IsNullOrEmpty(issuer))
                throw new ArgumentNullException(nameof(issuer), "Issuer cannot be null or empty.");
            if (string.IsNullOrEmpty(audience))
                throw new ArgumentNullException(nameof(audience), "Audience cannot be null or empty.");

            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        /// <summary>
        /// Generates a JWT token with the specified claims and expiry.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="userName">User name</param>
        /// <param name="email">Email address</param>
        /// <param name="role">User role</param>
        /// <param name="expiryInHours">Token expiry time in hours</param>
        /// <returns>A signed JWT token</returns>
        public string GenerateToken(string userId, string userName, string email, string role, int expiryInHours = 1)
        {
            if (expiryInHours <= 0)
                throw new ArgumentOutOfRangeException(nameof(expiryInHours), "Expiry time must be greater than 0.");

            // Create security key and credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Define claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique identifier for token
                new Claim("role", role) // Custom claim for user role
            };

            // Generate token
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiryInHours),
                signingCredentials: credentials
            );

            // Return serialized token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
