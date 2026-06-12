namespace api_gateway.Contracts.Attendance
{
    public record CheckInAttendanceRequest(
        Guid StudentId,
        double Confidence
    );
}