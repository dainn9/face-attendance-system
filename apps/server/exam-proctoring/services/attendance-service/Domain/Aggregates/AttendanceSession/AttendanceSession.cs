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

        public static AttendanceSession Create(Guid courseSectionId, DateTime now)
        {
            if (courseSectionId == Guid.Empty)
                throw new BusinessRuleViolationException("Course section ID cannot be empty.", ErrorCodes.InvalidAttendanceSessionData);

            // TODO: Support scheduled attendance sessions

            // if (startTime == default)
            //     throw new BusinessRuleViolationException("Start time must be provided.", ErrorCodes.InvalidAttendanceSessionData);

            // if (date == default)
            //     throw new BusinessRuleViolationException("Date must be provided.", ErrorCodes.InvalidAttendanceSessionData);

            // if (date < DateOnly.FromDateTime(now))
            //     throw new BusinessRuleViolationException("Attendance session date cannot be in the past.", ErrorCodes.InvalidAttendanceSessionData);

            // if (date == DateOnly.FromDateTime(now) && startTime < TimeOnly.FromDateTime(now))
            //     throw new BusinessRuleViolationException("Start time cannot be in the past.", ErrorCodes.InvalidAttendanceSessionData);

            var attendanceSession = new AttendanceSession
            {
                Id = Guid.NewGuid(),
                CourseSectionId = courseSectionId,
                Date = DateOnly.FromDateTime(now),
                StartTime = TimeOnly.FromDateTime(now),
                Status = AttendanceSessionStatus.Open
            };

            attendanceSession.SetCreated(now);
            attendanceSession.SetUpdated(now);
            return attendanceSession;
        }

        public void Close(DateTime now)
        {
            if (Status == AttendanceSessionStatus.Closed)
                throw new BusinessRuleViolationException("Attendance session is already closed.", ErrorCodes.InvalidAttendanceSessionData);

            var endTime = TimeOnly.FromDateTime(now);

            if (endTime <= StartTime)
                throw new BusinessRuleViolationException("End time must be after start time.", ErrorCodes.InvalidAttendanceSessionData);

            EndTime = endTime;
            Status = AttendanceSessionStatus.Closed;

            SetUpdated(now);
        }

        public void CheckInStudent(Guid studentId, DateTime now, double confidence)
        {
            if (Status == AttendanceSessionStatus.Closed)
                throw new BusinessRuleViolationException("Cannot check in to a closed attendance session.", ErrorCodes.InvalidAttendanceSessionData);

            if (_records.Any(r => r.StudentId == studentId))
                throw new BusinessRuleViolationException("Student has already checked in.", ErrorCodes.InvalidAttendanceSessionData);

            var record = AttendanceRecord.Present(Id, studentId, now, confidence);
            _records.Add(record);
            SetUpdated(now);
        }

        public void MarkAbsentStudents(IReadOnlyCollection<Guid> studentIds, DateTime now)
        {
            if (Status != AttendanceSessionStatus.Closed)
                throw new BusinessRuleViolationException("Cannot mark students as absent in an open attendance session.", ErrorCodes.InvalidAttendanceSessionData);

            var existingStudentIds = _records.Select(r => r.StudentId).ToHashSet();

            foreach (var studentId in studentIds)
            {
                if (existingStudentIds.Contains(studentId))
                    continue;

                var record = AttendanceRecord.Absent(Id, studentId);
                _records.Add(record);
            }

            SetUpdated(now);
        }
    }
}