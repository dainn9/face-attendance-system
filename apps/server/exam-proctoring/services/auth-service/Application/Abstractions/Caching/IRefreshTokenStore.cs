namespace auth_service.Application.Abstractions.Caching
{
    public interface IRefreshTokenStore
    {
        Task StoreRefreshTokenAsync(Guid userId, string refreshToken, TimeSpan ttl);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task<Guid?> ConsumeRefreshTokenAsync(string refreshToken);
    }
}