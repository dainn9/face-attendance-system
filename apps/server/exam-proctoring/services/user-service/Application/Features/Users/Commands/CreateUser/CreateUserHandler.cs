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
        private readonly IFacultyRepository _facultyRepository;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserHandler(IUserRepository userRepository, IFacultyRepository facultyRepository, IClock clock, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _facultyRepository = facultyRepository;
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
                        var majorId = request.MajorId!.Value;
                        if (!await _facultyRepository.ExistsByMajorIdAsync(majorId, cancellationToken))
                            throw new EntityNotFoundException("Major", majorId);

                        user.AddStudentProfile(majorId);
                    }
                    break;

                case UserRole.Lecturer:
                    {
                        var facultyId = request.FacultyId!.Value;
                        if (!await _facultyRepository.ExistsByIdAsync(facultyId, cancellationToken))
                            throw new EntityNotFoundException("Faculty", facultyId);

                        user.AddLecturerProfile(facultyId);
                    }
                    break;
            }

            _userRepository.Add(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}