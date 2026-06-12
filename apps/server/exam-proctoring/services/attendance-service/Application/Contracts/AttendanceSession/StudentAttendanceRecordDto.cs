using attendance_service.Domain.Enums;

namespace attendance_service.Application.Contracts.AttendanceSession
{
    public record StudentAttendanceRecordDto(
        Guid AttendanceSessionId,
        DateOnly Date,
        TimeOnly StartTime,
        AttendanceRecordStatus Status,
        double? ConfidenceScore,
        DateTime? CheckedInAt
    );
}