using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;
using user_service.Application.Abstractions.Persistence;

namespace user_service.Application.Features.Faculties.Commands.UpdateFaculty
{
    public class UpdateFacultyHandler : IRequestHandler<UpdateFacultyCommand>
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public UpdateFacultyHandler(IFacultyRepository facultyRepository, IUnitOfWork unitOfWork, IClock clock)
        {
            _facultyRepository = facultyRepository;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task Handle(UpdateFacultyCommand request, CancellationToken ct)
        {
            var faculty = await _facultyRepository.FindFacultyAsync(request.FacultyId, ct)
                ?? throw new EntityNotFoundException("Faculty", request.FacultyId);

            if (faculty.Code != request.Code &&
                await _facultyRepository.ExistsFacultyByCodeAsync(request.Code, request.FacultyId, ct))
                throw new BusinessRuleViolationException(
                    $"Faculty with code {request.Code} already exists.",
                    ErrorCodes.FacultyCodeAlreadyExists);

            if (faculty.Name != request.Name &&
                await _facultyRepository.ExistsFacultyByNameAsync(request.Name, request.FacultyId, ct))
                throw new BusinessRuleViolationException(
                    $"Faculty with name {request.Name} already exists.",
                    ErrorCodes.FacultyNameAlreadyExists);

            faculty.Update(request.Name, request.Code, _clock.UtcNow);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}