using attendance_service.Domain.Enums;
using BuildingBlocks.Exceptions;

namespace attendance_service.Domain.Aggregates.AttendanceSession
{
    public class AttendanceRecord
    {
        public Guid Id { get; private set; }
        public Guid StudentId { get; private set; }
        public Guid AttendanceSessionId { get; private set; }
        public AttendanceRecordStatus Status { get; private set; }
        public DateTime? CheckedInAt { get; private set; }
        public double? Confidence { get; private set; }

        private AttendanceRecord() { }

        public static AttendanceRecord Present(
           Guid attendanceSessionId,
           Guid studentId,
           DateTime checkedInAt,
           double confidence)
        {
            ValidateIds(attendanceSessionId, studentId);
            ValidateConfidence(confidence);

            return new AttendanceRecord
            {
                Id = Guid.NewGuid(),
                AttendanceSessionId = attendanceSessionId,
                StudentId = studentId,
                Status = AttendanceRecordStatus.Present,
                CheckedInAt = checkedInAt,
                Confidence = confidence
            };
        }

        public static AttendanceRecord Late(
            Guid attendanceSessionId,
            Guid studentId,
            DateTime checkedInAt,
            double confidence)
        {
            ValidateIds(attendanceSessionId, studentId);
            ValidateConfidence(confidence);

            return new AttendanceRecord
            {
                Id = Guid.NewGuid(),
                AttendanceSessionId = attendanceSessionId,
                StudentId = studentId,
                Status = AttendanceRecordStatus.Late,
                CheckedInAt = checkedInAt,
                Confidence = confidence
            };
        }

        public static AttendanceRecord Absent(
            Guid attendanceSessionId,
            Guid studentId)
        {
            ValidateIds(attendanceSessionId, studentId);

            return new AttendanceRecord
            {
                Id = Guid.NewGuid(),
                AttendanceSessionId = attendanceSessionId,
                StudentId = studentId,
                Status = AttendanceRecordStatus.Absent,
                CheckedInAt = null,
                Confidence = null
            };
        }

        private static void ValidateIds(Guid attendanceSessionId, Guid studentId)
        {
            if (attendanceSessionId == Guid.Empty)
                throw new BusinessRuleViolationException(
                    "Attendance session ID cannot be empty.",
                    ErrorCodes.InvalidAttendanceRecordData);

            if (studentId == Guid.Empty)
                throw new BusinessRuleViolationException(
                    "Student ID cannot be empty.",
                    ErrorCodes.InvalidAttendanceRecordData);
        }

        private static void ValidateConfidence(double confidence)
        {
            if (confidence is < 0 or > 1)
                throw new BusinessRuleViolationException(
                    "Confidence must be between 0 and 1.",
                    ErrorCodes.InvalidAttendanceRecordData);
        }
    }
}