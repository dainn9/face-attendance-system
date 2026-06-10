using System.Text.Json;
using auth_service.Application.Abstractions.Caching;
using auth_service.Application.Abstractions.Security;
using auth_service.Application.Contracts;
using auth_service.Domain.Enum;
using BuildingBlocks.Exceptions;
using StackExchange.Redis;

namespace auth_service.Infrastructure.Caching
{
    public class RedisRefreshTokenStore : IRefreshTokenStore
    {
        private readonly IDatabase _database;
        private readonly ITokenService _tokenService;
        private const string RefreshTokenKeyPrefix = "ep:auth:refresh_token:";
        private const string SessionKeyPrefix = "ep:auth:session:";

        public RedisRefreshTokenStore(RedisConnectionFactory factory, ITokenService tokenService)
        {
            _database = factory.GetDatabase();
            _tokenService = tokenService;
        }

        public async Task StoreRefreshTokenAsync(Guid userId, SessionType sessionType, string refreshToken, TimeSpan ttl)
        {
            var hashedToken = _tokenService.HashToken(refreshToken);
            var session = new RefreshSession(userId, sessionType);
            var json = JsonSerializer.Serialize(session);

            var batch = _database.CreateBatch();

            var t1 = batch.StringSetAsync(RefreshTokenKeyPrefix + hashedToken, json, ttl);

            var t2 = batch.StringSetAsync(SessionKeyPrefix + userId + ":" + sessionType, hashedToken, ttl);

            batch.Execute();
            await Task.WhenAll(t1, t2);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var hashedToken = _tokenService.HashToken(refreshToken);
            var session = await GetSessionByRefreshTokenAsync(refreshToken);

            var batch = _database.CreateBatch();

            var t1 = batch.KeyDeleteAsync(RefreshTokenKeyPrefix + hashedToken);

            var t2 = batch.KeyDeleteAsync(SessionKeyPrefix + session!.UserId + ":" + session.SessionType);

            batch.Execute();

            await Task.WhenAll(t1, t2);
        }

        public async Task ConsumeRefreshTokenAsync(string refreshToken)
        {
            var hashedToken = _tokenService.HashToken(refreshToken);
            var refreshKey = RefreshTokenKeyPrefix + hashedToken;

            var session = await GetSessionByRefreshTokenAsync(refreshToken);

            var sessionKey = SessionKeyPrefix + session.UserId + ":" + session.SessionType;
            var currentHashedToken = await _database.StringGetAsync(sessionKey);

            if (currentHashedToken.IsNullOrEmpty || currentHashedToken != hashedToken)
            {
                throw new UnauthorizedException(
                    "Refresh token already revoked.",
                    ErrorCodes.InvalidRefreshToken
                );
            }

            var batch = _database.CreateBatch();

            var t1 = batch.KeyDeleteAsync(refreshKey);
            var t2 = batch.KeyDeleteAsync(sessionKey);

            batch.Execute();
            await Task.WhenAll(t1, t2);
        }

        public async Task RevokeAllRefreshTokensAsync(Guid userId)
        {
            var tokenTypes = Enum.GetValues<SessionType>();

            var tasks = tokenTypes.Select(async t =>
            {
                var sessionKey = SessionKeyPrefix + userId + ":" + t;
                var hashedToken = await _database.StringGetDeleteAsync(sessionKey);

                if (!hashedToken.IsNullOrEmpty)
                {
                    await _database.KeyDeleteAsync(RefreshTokenKeyPrefix + hashedToken);
                }
            });

            await Task.WhenAll(tasks);
        }

        public async Task<RefreshSession> GetSessionByRefreshTokenAsync(string refreshToken)
        {
            var hashedToken = _tokenService.HashToken(refreshToken);
            var key = RefreshTokenKeyPrefix + hashedToken;

            var json = await _database.StringGetAsync(key);

            if (json.IsNullOrEmpty)
                throw new UnauthorizedException("Invalid refresh token.", ErrorCodes.InvalidRefreshToken);

            try
            {
                return JsonSerializer.Deserialize<RefreshSession>(json!)
                    ?? throw new UnauthorizedException("Invalid refresh token.", ErrorCodes.InvalidRefreshToken);
            }
            catch (JsonException)
            {
                throw new UnauthorizedException("Invalid refresh token.", ErrorCodes.InvalidRefreshToken);
            }
        }

        public async Task RevokeSessionsAsync(Guid userId, SessionType sessionType)
        {
            var sessionKey = SessionKeyPrefix + userId + ":" + sessionType;
            var oldHashedToken = await _database.StringGetDeleteAsync(sessionKey);

            if (!oldHashedToken.IsNullOrEmpty)
                await _database.KeyDeleteAsync(RefreshTokenKeyPrefix + oldHashedToken);
        }
    }
}