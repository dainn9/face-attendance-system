using attendance_service.Application.Contracts;
using BuildingBlocks.Results;

namespace attendance_service.Application.Abstractions.Persistence
{
    public interface ISubjectReadRepository
    {
        Task<SubjectDto?> GetByIdAsync(Guid subjectId, CancellationToken cancellationToken);
        Task<IReadOnlyList<SubjectLookupDto>> GetSubjectLookupAsync(string? keyword, CancellationToken cancellationToken);
        // Task<PagedResult<SubjectDto>> GetPagedAsync(int page, CancellationToken cancellationToken);
    }
}