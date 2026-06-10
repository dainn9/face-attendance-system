namespace attendance_service.API.Contracts.AttendanceSessions
{
    public record CheckInAttendanceRequest(
        Guid StudentId,
        double Confidence
    );
}