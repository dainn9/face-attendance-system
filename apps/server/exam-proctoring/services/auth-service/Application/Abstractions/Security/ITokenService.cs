using auth_service.Domain.Aggregates.User;
using auth_service.Domain.Enum;

namespace auth_service.Application.Abstractions.Security
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, SessionType sessionType);
        string GenerateRefreshToken();
        string HashToken(string token);
        TimeSpan GetAccessTokenExpiry();
        TimeSpan GetRefreshTokenExpiry();
    }
}