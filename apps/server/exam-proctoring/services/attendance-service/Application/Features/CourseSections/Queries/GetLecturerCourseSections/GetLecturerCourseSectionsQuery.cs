using attendance_service.Application.Contracts;
using attendance_service.Domain.Enums;
using BuildingBlocks.Results;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Queries.GetCoureseSectionPagedByLecturerId
{
    public record GetLecturerCourseSectionsQuery(
        Guid LecturerId,
        int Page = 1,
        int PageSize = 12,
        string? SearchQuery = null,
        Semester? Semester = null,
        string? AcademicYear = null,
        bool? IsActive = null
    ) : IRequest<PagedResult<LecturerCourseSectionDto>>;
}