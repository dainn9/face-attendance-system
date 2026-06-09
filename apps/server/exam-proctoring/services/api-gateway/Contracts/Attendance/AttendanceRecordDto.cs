namespace api_gateway.Contracts.Attendance
{
    public record AttendanceRecordDto
    (
        Guid StudentId,
        int Status,
        double? Confidence,
        DateTime? CheckedInAt
    );
}