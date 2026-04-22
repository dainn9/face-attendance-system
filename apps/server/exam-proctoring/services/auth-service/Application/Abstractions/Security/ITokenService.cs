using auth_service.Domain.Aggregates.User;

namespace auth_service.Application.Abstractions.Security
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string HashToken(string token);
        TimeSpan GetAccessTokenExpiry();
        TimeSpan GetRefreshTokenExpiry();
    }
}