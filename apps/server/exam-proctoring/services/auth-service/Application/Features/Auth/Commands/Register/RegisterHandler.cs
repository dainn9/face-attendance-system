using auth_service.Application.Abstractions.Persistence;
using auth_service.Application.Abstractions.Security;
using auth_service.Domain.Aggregates.User;
using auth_service.Domain.ValueObjects;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IClock _clock;

        public RegisterHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IClock clock)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _clock = clock;
        }

        public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
                throw new BusinessRuleViolationException("Email is already registered", ErrorCodes.EmailAlreadyExists);

            var hashedPassword = PasswordHash.Create(_passwordHasher.Hash(request.Password));

            var user = User.Create(email, hashedPassword, request.UserRole, _clock.UtcNow);

            await _userRepository.AddAsync(user, cancellationToken);
        }
    }
}