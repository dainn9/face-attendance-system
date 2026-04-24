using auth_service.Domain.Enum;

namespace auth_service.Application.Abstractions.Caching
{
    public interface IRefreshTokenStore
    {
        Task StoreRefreshTokenAsync(Guid userId, SessionType sessionType, string refreshToken, TimeSpan ttl);
        Task RevokeRefreshTokenAsync(Guid userId, SessionType sessionType);
        Task ConsumeRefreshTokenAsync(Guid userId, SessionType sessionType, string refreshToken);
        Task RevokeAllRefreshTokensAsync(Guid userId);
    }
}