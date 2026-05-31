using user_service.Application.Contracts;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IUserReadRepository
    {
        // Task<UserDto?> GetProfileByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Dictionary<Guid, int>> GetLecturerCountByFacultyAsync(CancellationToken ct = default);
        Task<Dictionary<Guid, int>> GetStudentCountByMajorsAsync(CancellationToken ct = default);

        Task<Dictionary<Guid, int>> GetStudentCountByFacultyIdAsync(Guid facultyId, CancellationToken ct = default);
        Task<IReadOnlyList<LecturerDto>> GetLecturersByFacultyIdAsync(Guid facultyId, CancellationToken ct = default);

    }
}