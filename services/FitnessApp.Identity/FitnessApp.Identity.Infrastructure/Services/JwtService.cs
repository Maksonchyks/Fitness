using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitnessApp.Identity.Domain.Entities;
using FitnessApp.Identity.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace FitnessApp.Identity.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GetSecret());

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim("userId", user.Id.ToString()),
            new Claim("email", user.Email.Value),
            new Claim("username", user.Username)
        };

            // Додаємо ролі
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
                Issuer = GetIssuer(),
                Audience = GetAudience(),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", "")
                .Replace("/", "");
        }

        public ClaimsPrincipal? ValidateJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GetSecret());

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = GetIssuer(),
                    ValidAudience = GetAudience(),
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public Guid? GetUserIdFromToken(string token)
        {
            var principal = ValidateJwtToken(token);
            var userIdClaim = principal?.FindFirst("userId")?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;

            return null;
        }

        private string GetSecret() => _configuration["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT Secret is not configured");

        private int GetTokenExpirationMinutes() =>
            int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var minutes)
                ? minutes : 60;

        private string GetIssuer() => _configuration["JwtSettings:Issuer"] ?? "FitnessApp.Identity";
        private string GetAudience() => _configuration["JwtSettings:Audience"] ?? "FitnessApp";
    }
}
