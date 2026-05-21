using BuildingBlocks.Time;
using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Domain.Aggregates.User;

namespace user_service.Application.Features.Users.Commands.CreateUser
{
    public class CreateUser : IRequestHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClock _clock;

        public CreateUser(IUserRepository userRepository, IClock clock)
        {
            _userRepository = userRepository;
            _clock = clock;
        }

        public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByIdAsync(request.UserId, cancellationToken))
                return;

            var user = User.Create(
                request.UserId,
                request.FullName,
                request.Gender,
                request.DateOfBirth,
                request.Email,
                _clock.UtcNow
            );

            await _userRepository.AddAsync(user, cancellationToken);
        }
    }
}