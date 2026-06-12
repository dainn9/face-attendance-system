using attendance_service.Domain.Enums;

namespace attendance_service.Application.Contracts.AttendanceSession
{
    public record AttendanceCheckInInfoDto(
        Guid Id,
        string SubjectName,
        string CourseSectionCode,
        DateOnly Date,
        TimeOnly StartTime,
        AttendanceSessionStatus Status
    );
}