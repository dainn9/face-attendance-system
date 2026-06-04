using attendance_service.Application.Contracts;
using attendance_service.Domain.Enums;
using BuildingBlocks.Results;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Queries.GetCourseSectionPaged
{
    public record GetCourseSectionPagedQuery(
        int Page = 1,
        int PageSize = 12,
        string? SearchQuery = null,
        Guid? FacultyId = null,
        Semester? Semester = null,
        string? AcademicYear = null,
        bool? IsActive = null
    ) : IRequest<PagedResult<CourseSectionPagedDto>>;
}