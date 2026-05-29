using user_service.Domain.Aggregates.Faculty;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IFacultyRepository
    {
        void Add(Faculty faculty);
        void Update(Faculty faculty);
        Task<bool> ExitsByNameAsync(string name, CancellationToken ct);
        Task<bool> ExitsByCodeAsync(string code, CancellationToken ct);
    }
}