using auth_service.Application.Abstractions.Persistence;
using auth_service.Application.Abstractions.Security;
using auth_service.Application.Abstractions.Services;
using auth_service.Domain.Aggregates.User;
using auth_service.Domain.ValueObjects;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;

namespace auth_service.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IClock _clock;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IClock clock)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _clock = clock;
        }

        public async Task<User> AuthenticateAsync(string email, string password, CancellationToken ct)
        {
            var emailObj = Email.Create(email);
            var user = await _userRepository.GetByEmailAsync(emailObj, ct)
                ?? throw new UnauthorizedException("Email or password not correct.", ErrorCodes.Unauthorized);

            if (!user.IsActive)
                throw new UnauthorizedException("Account is disabled.", ErrorCodes.AccountDisabled);

            if (user.IsLocked())
                throw new UnauthorizedException("Account is locked out.", ErrorCodes.AccountLocked);

            if (!_passwordHasher.Verify(password, user.PasswordHash.Value))
            {
                user.RecordFailedLogin(5, TimeSpan.FromMinutes(15), _clock.UtcNow);
                await _userRepository.UpdateAsync(user, ct);
                throw new UnauthorizedException("Email or password not correct.", ErrorCodes.Unauthorized);
            }

            user.RecordSuccessfulLogin(_clock.UtcNow);
            await _userRepository.UpdateAsync(user, ct);
            return user;
        }
    }
}