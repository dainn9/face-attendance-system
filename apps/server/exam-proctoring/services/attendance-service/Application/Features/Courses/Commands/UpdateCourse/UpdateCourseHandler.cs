using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Domain.Aggregates.Course;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;

namespace attendance_service.Application.Features.Courses.Commands.UpdateCourse
{
    public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public UpdateCourseHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IClock clock)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.FindAsync(request.CourseId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Course), request.CourseId);

            if (await _courseRepository.ExistsByCodeAsync(request.Code, request.CourseId, cancellationToken))
                throw new BusinessRuleViolationException($"Course with code '{request.Code}' already exists.", ErrorCodes.CourseCodeAlreadyExists);

            course.Update(
                request.Name,
                request.Code,
                request.Credits,
                _clock.UtcNow
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}