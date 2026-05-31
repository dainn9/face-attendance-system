using user_service.Domain.Aggregates.Faculty;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IFacultyRepository
    {
        void Add(Faculty faculty);
        Task<Faculty?> GetWithMajorsAsync(Guid facultyId, CancellationToken ct);

        Task<bool> ExistsFacultyByNameAsync(string name, CancellationToken ct);
        Task<bool> ExistsFacultyByCodeAsync(string code, CancellationToken ct);
        Task<bool> ExistsFacultyByIdAsync(Guid facultyId, CancellationToken ct);

        Task<bool> ExistsMajorByIdAsync(Guid majorId, CancellationToken ct);
    }
}