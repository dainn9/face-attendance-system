using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using MediatR;
using user_service.Application.Abstractions.Persistence;

namespace user_service.Application.Features.Majors.Commands.CreateMajor
{
    public class CreateMajorHandler : IRequestHandler<CreateMajorCommand>
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateMajorHandler(IUnitOfWork unitOfWork, IFacultyRepository facultyRepository)
        {
            _unitOfWork = unitOfWork;
            _facultyRepository = facultyRepository;
        }
        public async Task Handle(CreateMajorCommand request, CancellationToken cancellationToken)
        {
            var faculty = await _facultyRepository.GetWithMajorsAsync(request.FacultyId, cancellationToken)
                ?? throw new EntityNotFoundException("Faculty", request.FacultyId);

            faculty.AddMajor(request.Name, request.Code);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}