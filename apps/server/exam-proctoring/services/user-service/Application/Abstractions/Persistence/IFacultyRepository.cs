using user_service.Domain.Aggregates.Faculty;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IFacultyRepository
    {
        void Add(Faculty faculty);
        Task<Faculty?> FindFacultyAsync(Guid facultyId, CancellationToken ct);

        Task<Faculty?> GetFacultyWithMajorsAsync(Guid facultyId, CancellationToken ct);

        Task<bool> ExistsFacultyByNameAsync(string name, Guid? excludeId, CancellationToken ct);
        Task<bool> ExistsFacultyByCodeAsync(string code, Guid? excludeId, CancellationToken ct);
        Task<bool> ExistsFacultyByIdAsync(Guid facultyId, CancellationToken ct);

        Task<bool> ExistsMajorByIdAsync(Guid majorId, CancellationToken ct);
    }
}