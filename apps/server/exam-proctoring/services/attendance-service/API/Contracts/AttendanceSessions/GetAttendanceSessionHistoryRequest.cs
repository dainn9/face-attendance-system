namespace attendance_service.API.Contracts.AttendanceSessions
{
    public record GetAttendanceSessionHistoryRequest(
        int Page = 1,
        int PageSize = 10
    );
}