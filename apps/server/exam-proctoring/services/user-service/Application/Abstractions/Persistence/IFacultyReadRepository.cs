using user_service.Application.Contracts;
using user_service.Application.Contracts.Faculties;
using user_service.Application.Contracts.Majors;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IFacultyReadRepository
    {
        Task<IReadOnlyList<FacultyDto>> GetFacultiesAsync(CancellationToken ct = default);
        Task<FacultyDto?> GetFacultyByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<FacultyLookupDto>> GetFacultyLookupAsync(CancellationToken ct = default);
        Task<IReadOnlyList<MajorLookupDto>> GetMajorLookupByFacultyIdAsync(Guid facultyId, CancellationToken ct = default);
    }
}