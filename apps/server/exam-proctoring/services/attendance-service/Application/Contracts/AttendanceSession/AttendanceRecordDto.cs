using attendance_service.Domain.Enums;

namespace attendance_service.Application.Contracts.AttendanceSession
{
    public record AttendanceRecordDto(
        Guid StudentId,
        AttendanceRecordStatus Status,
        double? Confidence,
        DateTime? CheckedInAt
    );
}