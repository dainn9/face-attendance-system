using auth_service.Application.Abstractions.Caching;
using auth_service.Application.Abstractions.Security;
using BuildingBlocks.Exceptions;
using StackExchange.Redis;

namespace auth_service.Infrastructure.Caching
{
    public class RedisRefreshTokenStore : IRefreshTokenStore
    {
        private readonly IDatabase _database;
        private readonly ITokenService _tokenService;
        private const string KeyPrefix = "ep:auth:refresh_token:";

        public RedisRefreshTokenStore(RedisConnectionFactory factory, ITokenService tokenService)
        {
            _database = factory.GetDatabase();
            _tokenService = tokenService;
        }

        public async Task StoreRefreshTokenAsync(Guid userId, string refreshToken, TimeSpan ttl)
        {
            var hashedToken = _tokenService.HashToken(refreshToken);
            var result = await _database.StringSetAsync(KeyPrefix + hashedToken, userId.ToString(), ttl);

            if (!result)
                throw new RefreshTokenStoreException("Failed to store refresh token in Redis.");
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var hashedToken = _tokenService.HashToken(refreshToken);
            await _database.KeyDeleteAsync(KeyPrefix + hashedToken);
        }

        public async Task<Guid?> ConsumeRefreshTokenAsync(string refreshToken)
        {
            var hashedToken = _tokenService.HashToken(refreshToken);
            var value = await _database.StringGetDeleteAsync(KeyPrefix + hashedToken);

            if (value.IsNullOrEmpty)
                return null;

            if (!Guid.TryParse(value, out var userId))
                return null;

            return userId;
        }
    }
}