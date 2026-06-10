using Microsoft.EntityFrameworkCore;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts.Faculties;
using user_service.Application.Contracts.Majors;

namespace user_service.Infrastructure.Persistence.Repositories
{
    public class FacultyReadRepository : IFacultyReadRepository
    {
        private readonly UserDbContext _context;

        public FacultyReadRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<FacultyDto>> GetFacultiesAsync(CancellationToken ct = default)
        => await _context.Faculties
            .AsNoTracking()
            .Select(f => new FacultyDto(
                f.Id,
                f.Name,
                f.Code,
                f.Majors.Count,
                0,
                0,
                f.Majors.Select(m => new MajorDto(
                    m.Id,
                    m.Name,
                    m.Code,
                    0
                )).ToArray()
            ))
            .ToListAsync(ct);

        public Task<FacultyDto?> GetFacultyByIdAsync(Guid id, CancellationToken ct = default)
        => _context.Faculties
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new FacultyDto(
                f.Id,
                f.Name,
                f.Code,
                f.Majors.Count,
                0,
                0,
                f.Majors.Select(m => new MajorDto(
                    m.Id,
                    m.Name,
                    m.Code,
                    0
                )).ToArray()
            ))
            .FirstOrDefaultAsync(ct);

        public async Task<IReadOnlyList<FacultyLookupDto>> GetFacultyLookupAsync(CancellationToken ct = default)
        => await _context.Faculties
            .AsNoTracking()
            .Select(f => new FacultyLookupDto(
                f.Id,
                f.Name
            ))
            .ToListAsync(ct);

        public async Task<IReadOnlyList<MajorLookupDto>> GetMajorLookupByFacultyIdAsync(Guid facultyId, CancellationToken ct = default)
        => await _context.Faculties
            .AsNoTracking()
            .Where(f => f.Id == facultyId)
            .SelectMany(f => f.Majors)
            .OrderBy(m => m.Name)
            .Select(m => new MajorLookupDto(
                m.Id,
                m.Name
            ))
            .ToListAsync(ct);
    }
}