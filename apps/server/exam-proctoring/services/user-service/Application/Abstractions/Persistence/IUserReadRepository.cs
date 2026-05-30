using user_service.Application.Contracts;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IUserReadRepository
    {
        // Task<UserDto?> GetProfileByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Dictionary<Guid, int>> GetLecturerCountByFacultyAsync(CancellationToken cancellationToken = default);
        Task<Dictionary<Guid, int>> GetStudentCountByMajorsAsync(CancellationToken cancellationToken = default);
    }
}