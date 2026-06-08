namespace api_gateway.Contracts.Attendance
{
    public record LecturerCourseSectionStudentDto(
        Guid UserId,
        string StudentCode,
        string FullName,
        string Email,
        int PresentSessions,
        int TotalSessions,
        double AttendanceRate
    );
}