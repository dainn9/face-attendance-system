using attendance_service.Domain.Enums;
using BuildingBlocks.Exceptions;

namespace attendance_service.Domain.Aggregates.AttendanceSession
{
    public class AttendanceRecord
    {
        public Guid AttendanceSessionId { get; private set; }
        public Guid StudentId { get; private set; }
        public AttendanceRecordStatus Status { get; private set; }
        public DateTime? CheckedInAt { get; private set; }
        public double? Confidence { get; private set; }

        private AttendanceRecord() { }

        public static AttendanceRecord Create(Guid attendanceSessionId, Guid studentId, AttendanceRecordStatus status, DateTime? checkedInAt, double? confidence)
        {
            if (attendanceSessionId == Guid.Empty)
                throw new BusinessRuleViolationException("Attendance session ID cannot be empty.", ErrorCodes.InvalidAttendanceRecordData);

            if (studentId == Guid.Empty)
                throw new BusinessRuleViolationException("Student ID cannot be empty.", ErrorCodes.InvalidAttendanceRecordData);

            return new AttendanceRecord()
            {
                AttendanceSessionId = attendanceSessionId,
                StudentId = studentId,
                Status = status,
                CheckedInAt = checkedInAt,
                Confidence = confidence
            };
        }
    }
}