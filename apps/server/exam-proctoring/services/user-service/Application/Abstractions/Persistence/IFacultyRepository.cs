using user_service.Domain.Aggregates.Faculty;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IFacultyRepository
    {
        void Add(Faculty faculty);
        void Update(Faculty faculty);
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken ct);
        Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct);
        Task<bool> ExistsByMajorIdAsync(Guid majorId, CancellationToken ct);
    }
}