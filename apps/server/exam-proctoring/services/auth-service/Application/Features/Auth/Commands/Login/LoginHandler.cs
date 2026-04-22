using auth_service.Application.Abstractions.Caching;
using auth_service.Application.Abstractions.Persistence;
using auth_service.Application.Abstractions.Security;
using auth_service.Application.Contracts;
using auth_service.Domain.ValueObjects;
using BuildingBlocks.Exceptions;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenStore _refreshTokenStore;

        public LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService, IRefreshTokenStore refreshTokenStore)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _refreshTokenStore = refreshTokenStore;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken ct)
        {
            var email = Email.Create(request.Email);
            var user = await _userRepository.GetByEmailAsync(email, ct)
                ?? throw new UnauthorizedException("Email or password not correct.", ErrorCodes.Unauthorized);

            if (!user.IsActive)
                throw new UnauthorizedException("Account is disabled.", ErrorCodes.AccountDisabled);

            if (user.IsLocked())
                throw new UnauthorizedException("Account is locked out.", ErrorCodes.AccountLocked);

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash.Value))
            {
                user.RecordFailedLogin(5, TimeSpan.FromMinutes(15), DateTime.UtcNow);
                await _userRepository.UpdateAsync(user, ct);
                throw new UnauthorizedException("Email or password not correct.", ErrorCodes.Unauthorized);
            }

            user.RecordSuccessfulLogin(DateTime.UtcNow);
            await _userRepository.UpdateAsync(user, ct);

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