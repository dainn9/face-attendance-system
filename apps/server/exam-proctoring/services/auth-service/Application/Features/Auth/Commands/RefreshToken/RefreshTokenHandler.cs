using auth_service.Application.Abstractions.Caching;
using auth_service.Application.Abstractions.Persistence;
using auth_service.Application.Abstractions.Security;
using auth_service.Application.Contracts;
using BuildingBlocks.Exceptions;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly IUserRepository _userRepository;

        public RefreshTokenHandler(ITokenService tokenService, IRefreshTokenStore refreshTokenStore, IUserRepository userRepository)
        {
            _tokenService = tokenService;
            _refreshTokenStore = refreshTokenStore;
            _userRepository = userRepository;
        }

        public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            var userId = await _refreshTokenStore.ConsumeRefreshTokenAsync(request.RefreshToken)
                ?? throw new UnauthorizedException("Invalid refresh token.", ErrorCodes.InvalidRefreshToken);

            var user = await _userRepository.GetByIdAsync(userId, ct)
                ?? throw new UnauthorizedException("Invalid refresh token.", ErrorCodes.InvalidRefreshToken);

            if (!user.IsActive)
                throw new UnauthorizedException("Invalid refresh token.", ErrorCodes.AccountDisabled);

            if (user.IsLocked())
                throw new UnauthorizedException("Invalid refresh token.", ErrorCodes.AccountLocked);

            var accessTokenExpiry = _tokenService.GetAccessTokenExpiry();
            var refreshTokenExpiry = _tokenService.GetRefreshTokenExpiry();

            var token = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _refreshTokenStore.StoreRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry);

            return new AuthResponse(
                AccessToken: token,
                RefreshToken: refreshToken,
                AccessTokenExpiresIn: accessTokenExpiry,
                RefreshTokenExpiresIn: refreshTokenExpiry
            );
        }
    }
}