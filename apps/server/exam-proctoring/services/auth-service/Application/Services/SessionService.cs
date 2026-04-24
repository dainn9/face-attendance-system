using auth_service.Application.Abstractions.Caching;
using auth_service.Application.Abstractions.Security;
using auth_service.Application.Abstractions.Services;
using auth_service.Application.Contracts;
using auth_service.Domain.Aggregates.User;
using auth_service.Domain.Enum;

namespace auth_service.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly ITokenService _tokenService;

        public SessionService(IRefreshTokenStore refreshTokenStore, ITokenService tokenService)
        {
            _refreshTokenStore = refreshTokenStore;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> CreateSessionAsync(User user, SessionType sessionType, CancellationToken ct)
        {
            await _refreshTokenStore.RevokeRefreshTokenAsync(user.Id, sessionType);

            var accessTokenExpiry = _tokenService.GetAccessTokenExpiry();
            var refreshTokenExpiry = _tokenService.GetRefreshTokenExpiry();
            var token = _tokenService.GenerateAccessToken(user, sessionType);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _refreshTokenStore.StoreRefreshTokenAsync(user.Id, sessionType, refreshToken, refreshTokenExpiry);

            return new AuthResponse(
                AccessToken: token,
                RefreshToken: refreshToken,
                AccessTokenExpiresIn: accessTokenExpiry,
                RefreshTokenExpiresIn: refreshTokenExpiry
            );
        }
    }
}