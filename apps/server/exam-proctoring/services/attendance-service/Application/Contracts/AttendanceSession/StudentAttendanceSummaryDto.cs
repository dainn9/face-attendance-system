namespace attendance_service.Application.Contracts.AttendanceSession
{
    public record StudentAttendanceSummaryDto(
        Guid StudentId,
        int PresentSessions,
        int TotalSessions,
        double AttendanceRate
    );
}