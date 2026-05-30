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

        public Task<bool> ExistsByCodeAsync(string code, CancellationToken ct)
        => _context.Faculties.AsNoTracking().AnyAsync(f => f.Code == code, ct);

        public Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
        => _context.Faculties.AsNoTracking().AnyAsync(f => f.Name == name, ct);

        public void Update(Faculty faculty)
        => _context.Faculties.Update(faculty);

        public Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct)
        => _context.Faculties.AsNoTracking().AnyAsync(f => f.Id == id, ct);

        public Task<bool> ExistsByMajorIdAsync(Guid majorId, CancellationToken ct)
        => _context.Faculties.AsNoTracking().AnyAsync(f => f.Majors.Any(m => m.Id == majorId), ct);
    }
}