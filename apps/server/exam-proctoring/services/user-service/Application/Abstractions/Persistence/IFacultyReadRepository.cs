using user_service.Application.Contracts;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IFacultyReadRepository
    {
        Task<IReadOnlyList<FacultyDto>> GetFacultiesAsync(CancellationToken cancellationToken = default);
    }
}