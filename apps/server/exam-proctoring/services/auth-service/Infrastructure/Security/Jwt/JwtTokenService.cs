using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using auth_service.Application.Abstractions.Security;
using auth_service.Domain.Aggregates.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace auth_service.Infrastructure.Security.Jwt
{
    public class JwtTokenService : ITokenService
    {
        private readonly IJwtKeyProvider _keyProvider;
        private readonly JwtOptions _options;

        public JwtTokenService(IJwtKeyProvider keyProvider, IOptions<JwtOptions> options)
        {
            _keyProvider = keyProvider;
            _options = options.Value;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public string GenerateAccessToken(User user)
        {
            var creds = _keyProvider.GetSigningCredentials();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email.Value),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes),
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }

        public string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashedBytes);
        }

        public TimeSpan GetAccessTokenExpiry()
        {
            return TimeSpan.FromMinutes(_options.AccessTokenMinutes);
        }

        public TimeSpan GetRefreshTokenExpiry()
        {
            return TimeSpan.FromDays(_options.RefreshTokenDays);
        }

    }
}