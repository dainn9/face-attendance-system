namespace api_gateway.Contracts.Attendance
{
    public record GetCourseSectionPagedRequest(
        int Page = 1,
        int PageSize = 12,
        string? SearchQuery = null,
        Guid? FacultyId = null,
        string? Semester = null,
        string? AcademicYear = null,
        bool? IsActive = null
    );
}