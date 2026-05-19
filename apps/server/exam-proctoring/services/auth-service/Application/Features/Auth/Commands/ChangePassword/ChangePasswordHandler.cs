using auth_service.Application.Abstractions.Caching;
using auth_service.Application.Abstractions.Persistence;
using auth_service.Application.Abstractions.Security;
using auth_service.Application.Abstractions.System;
using auth_service.Domain.ValueObjects;
using BuildingBlocks.Exceptions;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly IClock _clock;

        public ChangePasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IRefreshTokenStore refreshTokenStore, IClock clock)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenStore = refreshTokenStore;
            _clock = clock;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId)
                ?? throw new EntityNotFoundException("User not found");

            if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash.Value))
                throw new UnauthorizedException("Current password is incorrect", ErrorCodes.PasswordIncorrect);

            var newHashedPassword = _passwordHasher.Hash(request.NewPassword);
            user.ChangePassword(PasswordHash.Create(newHashedPassword), _clock.UtcNow);

            await _userRepository.UpdateAsync(user, ct);
            await _refreshTokenStore.RevokeAllRefreshTokensAsync(request.UserId);
        }
    }
}