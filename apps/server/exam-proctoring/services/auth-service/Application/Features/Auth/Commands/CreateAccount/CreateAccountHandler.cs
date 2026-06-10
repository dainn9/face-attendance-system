using System.Text.Json;
using auth_service.Application.Abstractions.Persistence;
using auth_service.Application.Abstractions.Security;
using auth_service.Domain.Aggregates.User;
using auth_service.Domain.ValueObjects;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.CreateAccount
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAccountHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IClock clock, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _clock = clock;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
                throw new BusinessRuleViolationException("Email is already registered", ErrorCodes.EmailAlreadyExists);

            var hashedPassword = PasswordHash.Create(_passwordHasher.Hash(request.Password));

            var user = User.Create(email, hashedPassword, request.UserRole, _clock.UtcNow);

            _userRepository.Add(user);

            // var integrationEvent = new UserRegisteredIntegrationEvent(
            //     user.Id,
            //     request.FullName,
            //     request.Gender,
            //     request.DateOfBirth,
            //     user.Email.Value,
            //     request.UserRole,
            //     request.StudentCode,
            //     request.ClassCode,
            //     request.FacultyCode

            // );

            // var outboxMessage = OutboxMessage.Create(
            //     nameof(UserRegisteredIntegrationEvent),
            //     JsonSerializer.Serialize(integrationEvent),
            //     _clock.UtcNow
            // );

            // _outboxRepository.Add(outboxMessage);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return user.Id;
        }
    }
}
