using auth_service.Application.Abstractions.Caching;
using auth_service.Application.Abstractions.Security;
using auth_service.Domain.Enum;
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

        public async Task StoreRefreshTokenAsync(Guid userId, SessionType sessionType, string refreshToken, TimeSpan ttl)
        {
            var hashedToken = _tokenService.HashToken(refreshToken);
            var result = await _database.StringSetAsync(KeyPrefix + userId + ":" + sessionType, hashedToken, ttl);

            if (!result)
                throw new RefreshTokenStoreException("Failed to store refresh token in Redis.");
        }

        public async Task RevokeRefreshTokenAsync(Guid userId, SessionType sessionType)
        {
            await _database.KeyDeleteAsync(KeyPrefix + userId + ":" + sessionType);
        }

        public async Task ConsumeRefreshTokenAsync(Guid userId, SessionType sessionType, string refreshToken)
        {
            var key = KeyPrefix + userId + ":" + sessionType;
            var hashedToken = _tokenService.HashToken(refreshToken);

            var tran = _database.CreateTransaction();
            tran.AddCondition(Condition.StringEqual(key, hashedToken));
            _ = tran.KeyDeleteAsync(key);

            var committed = await tran.ExecuteAsync();
            if (!committed)
                throw new UnauthorizedException("Invalid refresh token.", ErrorCodes.InvalidRefreshToken);
        }
        public async Task RevokeAllRefreshTokensAsync(Guid userId)
        {
            var tokenTypes = Enum.GetValues<SessionType>();
            var tasks = tokenTypes.Select(t =>
                _database.KeyDeleteAsync(KeyPrefix + userId + ":" + t)
            );
            await Task.WhenAll(tasks);
        }
    }
}