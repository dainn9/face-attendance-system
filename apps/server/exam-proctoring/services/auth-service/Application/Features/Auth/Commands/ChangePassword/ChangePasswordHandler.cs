using auth_service.Application.Abstractions.Persistence;
using auth_service.Application.Abstractions.Security;
using auth_service.Domain.ValueObjects;
using BuildingBlocks.Exceptions;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Task>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public ChangePasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Task> Handle(ChangePasswordCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId)
                ?? throw new EntityNotFoundException("User not found");

            if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash.Value))
                throw new UnauthorizedException("Current password is incorrect", ErrorCodes.PasswordIncorrect);

            var newHashedPassword = _passwordHasher.Hash(request.NewPassword);
            user.ChangePassword(PasswordHash.Create(newHashedPassword), DateTime.UtcNow);

            await _userRepository.UpdateAsync(user, ct);
            return Task.CompletedTask;
        }
    }
}