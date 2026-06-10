namespace api_gateway.Contracts.Attendance
{
    public record AttendanceSessionStudentDto(
        Guid UserId,
        string StudentCode,
        string FullName,
        string Email,
        int? AttendanceStatus,
        double? Confidence,
        DateTime? CheckedInAt
    );
}