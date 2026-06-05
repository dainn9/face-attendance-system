using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Domain.Aggregates.CourseSection;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Commands.CreateCourseSection
{
    public class CreateCourseSectionHandler : IRequestHandler<CreateCourseSectionCommand, Guid>
    {
        private readonly ICourseSectionRepository _courseSectionRepository;
        private readonly ICourseSectionReadRepository _courseSectionReadRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;


        public CreateCourseSectionHandler(ICourseSectionRepository courseSectionRepository, ICourseSectionReadRepository courseSectionReadRepository, ISubjectRepository subjectRepository, IUnitOfWork unitOfWork, IClock clock)
        {
            _courseSectionRepository = courseSectionRepository;
            _courseSectionReadRepository = courseSectionReadRepository;
            _subjectRepository = subjectRepository;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task<Guid> Handle(CreateCourseSectionCommand request, CancellationToken cancellationToken)
        {
            var normalizedCode = request.CourseSectionCode.Trim().ToUpperInvariant();

            if (await _courseSectionReadRepository.ExistsByCodeAsync(normalizedCode, null, cancellationToken))
                throw new BusinessRuleViolationException($"Course section with code '{normalizedCode}' already exists.", ErrorCodes.CourseSectionCodeAlreadyExists);

            if (!await _subjectRepository.ExistsByIdAsync(request.SubjectId, cancellationToken))
                throw new EntityNotFoundException("Subject", request.SubjectId);

            var courseSection = CourseSection.Create(
                request.SubjectId,
                normalizedCode,
                request.Semester,
                request.AcademicYear,
                request.LecturerId,
                request.MaxCapacity,
                _clock.UtcNow
            );

            if (!request.Schedules.Any())
                throw new BusinessRuleViolationException("Course section must have at least one schedule.", ErrorCodes.CourseSectionMustHaveSchedule);

            var conflict = await _courseSectionReadRepository.GetRoomScheduleConflictAsync(request.Schedules, null, cancellationToken);

            if (conflict != null)
                throw new BusinessRuleViolationException(
                    $"Room '{conflict.Room}' is already booked for {conflict.CourseSectionCode} on {conflict.DayOfWeek} {conflict.StartTime}-{conflict.EndTime}.",
                    ErrorCodes.ScheduleConflict);

            foreach (var schedule in request.Schedules)
            {
                courseSection.AddSchedule(
                    schedule.DayOfWeek,
                    schedule.StartTime,
                    schedule.EndTime,
                    schedule.Room,
                    _clock.UtcNow
                );
            }

            _courseSectionRepository.Add(courseSection);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return courseSection.Id;
        }
    }
}