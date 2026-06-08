using BuildingBlocks.Results;
using SharedKernel.Core.Enums;
using user_service.Application.Contracts.Lecturers;
using user_service.Application.Contracts.Users;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IUserReadRepository
    {
        Task<UserDto?> GetProfileByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Dictionary<Guid, int>> GetLecturerCountByFacultyAsync(CancellationToken ct = default);
        Task<Dictionary<Guid, int>> GetStudentCountByMajorsAsync(CancellationToken ct = default);

        Task<Dictionary<Guid, int>> GetStudentCountByFacultyIdAsync(Guid facultyId, CancellationToken ct = default);
        Task<IReadOnlyList<LecturerDto>> GetLecturersByFacultyIdAsync(Guid facultyId, CancellationToken ct = default);

        Task<PagedResult<UserPagedDto>> GetPagedAsync(
            int page,
            int pageSize,
            string? searchQuery = null,
            UserRole? role = null,
            Guid? facultyId = null,
            CancellationToken ct = default
        );

        Task<Dictionary<Guid, UserLookupDto>> GetLecturersByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken);

        Task<IReadOnlyList<UserLookupDto>> GetLecturerLookupByFacultyIdAsync(Guid? facultyId, string? keyword, CancellationToken cancellationToken);

        Task<bool> CheckLecturerExistsByIdAsync(Guid lecturerId, CancellationToken cancellationToken);
        Task<LecturerDto?> GetLecturerByIdAsync(Guid lecturerId, CancellationToken cancellationToken);
    }
}