using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;
using SharedKernel.Core.Enums;
using user_service.Application.Abstractions.Persistence;
using user_service.Domain.Aggregates.User;

namespace user_service.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserHandler(IUserRepository userRepository, IClock clock, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _clock = clock;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByIdAsync(request.UserId, cancellationToken))
                return;

            if (await _userRepository.ExistsByUserCodeAsync(request.UserCode, cancellationToken))
                throw new BusinessRuleViolationException($"User code {request.UserCode} already exists.", ErrorCodes.UserCodeAlreadyExists);

            var user = User.Create(
                request.UserId,
                request.UserCode,
                request.FullName,
                request.Gender,
                request.DateOfBirth,
                request.Email,
                request.Role,
                _clock.UtcNow
            );

            switch (request.Role)
            {
                case UserRole.Student:
                    {
                        user.AddStudentProfile(
                            request.MajorId!.Value
                        );
                    }
                    break;

                case UserRole.Lecturer:
                    user.AddLecturerProfile(
                        request.FacultyId!.Value
                    );
                    break;
            }

            _userRepository.Add(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}