using attendance_service.Domain.Enums;

namespace attendance_service.API.Contracts.CourseSections
{
    public record GetCourseSectionPagedRequest(
        int Page = 1,
        int PageSize = 12,
        string? SearchQuery = null,
        Guid? FacultyId = null,
        Semester? Semester = null,
        string? AcademicYear = null,
        bool? IsActive = null
    );
}