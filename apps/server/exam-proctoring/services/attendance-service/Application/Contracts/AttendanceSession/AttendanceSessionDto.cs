using attendance_service.Domain.Enums;

namespace attendance_service.Application.Contracts.AttendanceSession
{
    public record AttendanceSessionDetailDto(
        Guid Id,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly? EndTime,
        AttendanceSessionStatus Status,
        int PresentCount,
        int AbsentCount,
        double AttendanceRate
    );
}