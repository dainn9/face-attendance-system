using Microsoft.EntityFrameworkCore;
using user_service.Application.Abstractions.Persistence;
using user_service.Domain.Aggregates.Faculty;

namespace user_service.Infrastructure.Persistence.Repositories
{
    public class FacultyRepository : IFacultyRepository
    {
        private readonly UserDbContext _context;

        public FacultyRepository(UserDbContext context)
        {
            _context = context;
        }

        public void Add(Faculty faculty)
        => _context.Faculties.Add(faculty);

        public async Task<Faculty?> FindFacultyAsync(Guid facultyId, CancellationToken ct)
        => await _context.Faculties.FindAsync([facultyId], ct);

        public async Task<Faculty?> GetFacultyWithMajorsAsync(Guid facultyId, CancellationToken ct)
        => await _context.Faculties.Include(f => f.Majors).FirstOrDefaultAsync(f => f.Id == facultyId, ct);

        public Task<bool> ExistsFacultyByCodeAsync(string code, Guid? excludeId, CancellationToken ct)
        => _context.Faculties.AsNoTracking().AnyAsync(f => f.Code == code && (excludeId == null || f.Id != excludeId), ct);

        public Task<bool> ExistsFacultyByNameAsync(string name, Guid? excludeId, CancellationToken ct)
        => _context.Faculties.AsNoTracking().AnyAsync(f => f.Name == name && (excludeId == null || f.Id != excludeId), ct);

        public Task<bool> ExistsFacultyByIdAsync(Guid id, CancellationToken ct)
        => _context.Faculties.AsNoTracking().AnyAsync(f => f.Id == id, ct);

        public Task<bool> ExistsMajorByIdAsync(Guid majorId, CancellationToken ct)
        => _context.Faculties.AsNoTracking().AnyAsync(f => f.Majors.Any(m => m.Id == majorId), ct);
    }
}