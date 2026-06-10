using attendance_service.Application.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;

namespace attendance_service.Application.Features.Enrollments.Commands.AddEnrollments
{
    public class AddEnrollmentsHandler : IRequestHandler<AddEnrollmentsCommand>
    {
        private readonly ICourseSectionRepository _courseSectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public AddEnrollmentsHandler(ICourseSectionRepository courseSectionRepository, IUnitOfWork unitOfWork, IClock clock)
        {
            _courseSectionRepository = courseSectionRepository;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task Handle(AddEnrollmentsCommand request, CancellationToken cancellationToken)
        {
            var courseSection = await _courseSectionRepository.GetWithEnrollmentsByIdAsync(request.CourseSectionId, cancellationToken)
                ?? throw new EntityNotFoundException("Course section", request.CourseSectionId);

            courseSection.AddEnrollments(request.StudentIds, _clock.UtcNow);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}