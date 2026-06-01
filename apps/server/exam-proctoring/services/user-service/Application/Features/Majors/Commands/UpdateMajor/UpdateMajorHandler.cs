using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using MediatR;
using user_service.Application.Abstractions.Persistence;

namespace user_service.Application.Features.Majors.Commands.UpdateMajor
{
    public class UpdateMajorHandler : IRequestHandler<UpdateMajorCommand>
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMajorHandler(IFacultyRepository facultyRepository, IUnitOfWork unitOfWork)
        {
            _facultyRepository = facultyRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(UpdateMajorCommand request, CancellationToken ct)
        {
            var faculty = await _facultyRepository.GetFacultyWithMajorsAsync(request.FacultyId, ct)
                ?? throw new EntityNotFoundException("Faculty", request.FacultyId);

            faculty.UpdateMajor(request.MajorId, request.Name, request.Code);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}