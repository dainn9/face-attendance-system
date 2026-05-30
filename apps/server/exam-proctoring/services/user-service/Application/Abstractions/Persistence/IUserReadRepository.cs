using user_service.Application.Contracts;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IUserReadRepository
    {
        // Task<UserDto?> GetProfileByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<int> GetStudentCountByFacultyIdAsync(Guid facultyId, CancellationToken cancellationToken = default);
    }
}