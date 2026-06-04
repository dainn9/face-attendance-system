using attendance_service.Domain.Enums;
using BuildingBlocks.Exceptions;
using SharedKernel.Core;

namespace attendance_service.Domain.Aggregates.AttendanceSession
{
    public class AttendanceSession : AggregateRoot<Guid>
    {
        public Guid CourseSectionId { get; private set; }
        public DateOnly Date { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly? EndTime { get; private set; }
        public AttendanceSessionStatus Status { get; private set; }

        private readonly List<AttendanceRecord> _records = new();
        public IReadOnlyCollection<AttendanceRecord> Records => _records.AsReadOnly();

        private AttendanceSession() { }

        public static AttendanceSession Create(Guid courseSectionId, DateOnly date, TimeOnly startTime, DateTime now)
        {
            if (courseSectionId == Guid.Empty)
                throw new BusinessRuleViolationException("Course section ID cannot be empty.", ErrorCodes.InvalidAttendanceSessionData);

            if (startTime == default)
                throw new BusinessRuleViolationException("Start time must be provided.", ErrorCodes.InvalidAttendanceSessionData);

            if (date == default)
                throw new BusinessRuleViolationException("Date must be provided.", ErrorCodes.InvalidAttendanceSessionData);

            if (date < DateOnly.FromDateTime(now))
                throw new BusinessRuleViolationException("Attendance session date cannot be in the past.", ErrorCodes.InvalidAttendanceSessionData);

            if (date == DateOnly.FromDateTime(now) && startTime < TimeOnly.FromDateTime(now))
                throw new BusinessRuleViolationException("Start time cannot be in the past.", ErrorCodes.InvalidAttendanceSessionData);

            return new AttendanceSession
            {
                Id = Guid.NewGuid(),
                CourseSectionId = courseSectionId,
                Date = date,
                StartTime = startTime,
                Status = AttendanceSessionStatus.Open
            };
        }

        public void Close(DateTime now)
        {
            if (Status == AttendanceSessionStatus.Closed)
                throw new BusinessRuleViolationException("Attendance session is already closed.", ErrorCodes.InvalidAttendanceSessionData);

            if (EndTime <= StartTime)
                throw new BusinessRuleViolationException("End time must be after start time.", ErrorCodes.InvalidAttendanceSessionData);

            EndTime = TimeOnly.FromDateTime(now);
            Status = AttendanceSessionStatus.Closed;
        }
    }
}