using attendance_service.Domain.Enums;

namespace attendance_service.API.Contracts.CourseSections
{
    public record GetLecturerCourseSectionRequest(
        int Page = 1,
        int PageSize = 12,
        string? SearchQuery = null,
        Semester? Semester = null,
        string? AcademicYear = null,
        bool? IsActive = null
    );
}