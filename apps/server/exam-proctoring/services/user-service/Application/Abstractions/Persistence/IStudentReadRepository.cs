using user_service.Application.Contracts.Students;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IStudentReadRepository
    {
        Task<Dictionary<Guid, StudentSummaryDto>> GetStudentSummariesByIdsAsync(
            IEnumerable<Guid> studentIds,
            CancellationToken cancellationToken
        );

        Task<IReadOnlyList<StudentLookupDto>> GetStudentLookupAsync(
            string? keyword,
            CancellationToken cancellationToken
        );
    }
}