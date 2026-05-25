using attendance_service.Application.Contracts;
using BuildingBlocks.Results;

namespace attendance_service.Application.Abstractions.Persistence
{
    public interface ICourseReadRepository
    {
        Task<CourseDto?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<PagedResult<CourseDto>> GetPagedAsync(int page, CancellationToken cancellationToken);
    }
}