using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Domain.Aggregates.Course;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;

namespace attendance_service.Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Guid>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public CreateCourseHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, IClock clock)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            if (await _courseRepository.ExistsByCodeAsync(request.Code, null, cancellationToken))
                throw new BusinessRuleViolationException($"Course with code '{request.Code}' already exists.", ErrorCodes.CourseCodeAlreadyExists);

            var course = Course.Create(
                request.Name,
                request.Code,
                request.Credits,
                _clock.UtcNow
            );

            _courseRepository.Add(course);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return course.Id;
        }
    }
}