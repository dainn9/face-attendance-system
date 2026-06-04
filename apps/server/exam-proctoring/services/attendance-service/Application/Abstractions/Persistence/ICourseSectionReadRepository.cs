using attendance_service.Application.Contracts;
using attendance_service.Domain.Enums;
using BuildingBlocks.Results;

namespace attendance_service.Application.Abstractions.Persistence
{
    public interface ICourseSectionReadRepository
    {
        Task<PagedResult<CourseSectionPagedDto>> GetCourseSectionsPagedAsync(
            int page = 1,
            int pageSize = 12,
            string? searchQuery = null,
            Guid? facultyId = null,
            Semester? semester = null,
            string? academicYear = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default
        );
    }
}