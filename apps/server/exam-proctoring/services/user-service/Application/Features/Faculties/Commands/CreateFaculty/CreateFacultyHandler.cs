using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Domain.Aggregates.Faculty;

namespace user_service.Application.Features.Faculties.Commands.CreateFaculty
{
    public class CreateFacultyHandler : IRequestHandler<CreateFacultyCommand, Guid>
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock clock;

        public CreateFacultyHandler(IFacultyRepository facultyRepository, IUnitOfWork unitOfWork, IClock clock)
        {
            _facultyRepository = facultyRepository;
            _unitOfWork = unitOfWork;
            this.clock = clock;
        }

        public async Task<Guid> Handle(CreateFacultyCommand request, CancellationToken cancellationToken)
        {
            if (await _facultyRepository.ExistsByNameAsync(request.Name, cancellationToken))
                throw new BusinessRuleViolationException("Faculty name already exists.", ErrorCodes.FacultyNameAlreadyExists);

            if (await _facultyRepository.ExistsByCodeAsync(request.Code, cancellationToken))
                throw new BusinessRuleViolationException("Faculty code already exists.", ErrorCodes.FacultyCodeAlreadyExists);

            var faculty = Faculty.Create(request.Name, request.Code, clock.UtcNow);

            _facultyRepository.Add(faculty);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return faculty.Id;
        }
    }
}