using auth_service.Application.Contracts;
using auth_service.Domain.Enum;

namespace auth_service.Application.Abstractions.Caching
{
    public interface IRefreshTokenStore
    {
        Task StoreRefreshTokenAsync(Guid userId, SessionType sessionType, string refreshToken, TimeSpan ttl);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task ConsumeRefreshTokenAsync(string refreshToken);
        Task RevokeAllRefreshTokensAsync(Guid userId);
        Task<RefreshSession> GetSessionByRefreshTokenAsync(string refreshToken);
        Task RevokeSessionsAsync(Guid userId, SessionType sessionType);
    }
}