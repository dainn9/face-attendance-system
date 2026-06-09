using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Domain.Aggregates.AttendanceSession;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Time;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Commands.CreateAttendanceSession
{
    public class CreateAttendanceSessionHandler : IRequestHandler<CreateAttendanceSessionCommand, Guid>
    {
        private readonly IAttendanceSessionRepository _attendanceSessionRepository;
        private readonly IAttendanceSessionReadRepository _attendanceSessionReadRepository;
        private readonly ICourseSectionRepository _courseSectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public CreateAttendanceSessionHandler(IAttendanceSessionRepository attendanceSessionRepository, IAttendanceSessionReadRepository attendanceSessionReadRepository, ICourseSectionRepository courseSectionRepository, IUnitOfWork unitOfWork, IClock clock)
        {
            _attendanceSessionRepository = attendanceSessionRepository;
            _attendanceSessionReadRepository = attendanceSessionReadRepository;
            _courseSectionRepository = courseSectionRepository;
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public async Task<Guid> Handle(CreateAttendanceSessionCommand request, CancellationToken cancellationToken)
        {
            var courseSection = await _courseSectionRepository.GetByIdAsync(request.CourseSectionId, cancellationToken)
                ?? throw new EntityNotFoundException("Course section", request.CourseSectionId);

            if (courseSection.LecturerId != request.LecturerId)
                throw new BusinessRuleViolationException(
                    "Only the lecturer of the course section can create an attendance session.",
                    ErrorCodes.Unauthorized
                );

            if (courseSection.Enrollments.Count == 0)
                throw new BusinessRuleViolationException(
                    "Cannot create attendance session for a course section with no enrollments.",
                    ErrorCodes.CourseSectionHasNoEnrollments
                );

            var now = _clock.UtcNow;
            var currentTime = TimeOnly.FromDateTime(now);
            var today = now.DayOfWeek;

            if (await _attendanceSessionReadRepository.HasOpenSessionAsync(request.CourseSectionId, cancellationToken))
                throw new BusinessRuleViolationException(
                    "There is already an open attendance session for this course section.",
                    ErrorCodes.AttendanceSessionAlreadyOpen);

            var hasValidScheduleNow = courseSection.Schedules.Any(s =>
                s.DayOfWeek == today &&
                s.StartTime <= currentTime &&
                s.EndTime >= currentTime);

            if (!hasValidScheduleNow)
                throw new BusinessRuleViolationException(
                    "Cannot create attendance session outside the course section schedule.",
                    ErrorCodes.CourseSectionScheduleNotMatched);

            var attendanceSession = AttendanceSession.Create(
                request.CourseSectionId,
                now
            );

            _attendanceSessionRepository.Add(attendanceSession);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return attendanceSession.Id;
        }
    }
}