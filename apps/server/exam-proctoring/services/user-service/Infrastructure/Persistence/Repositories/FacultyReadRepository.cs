using Microsoft.EntityFrameworkCore;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts;

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
    }
}