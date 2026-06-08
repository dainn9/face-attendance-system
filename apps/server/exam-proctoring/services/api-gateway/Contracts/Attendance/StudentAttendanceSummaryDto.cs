namespace api_gateway.Contracts.Attendance
{
    public record StudentAttendanceSummaryDto(
        Guid UserId,
        int PresentSessions,
        int TotalSessions,
        double AttendanceRate
    );
}