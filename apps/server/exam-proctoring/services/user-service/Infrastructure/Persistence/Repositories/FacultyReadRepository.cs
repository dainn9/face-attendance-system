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

        public async Task<IReadOnlyList<FacultyDto>> GetFacultiesAsync(CancellationToken cancellationToken = default)
        => await _context.Faculties
            .AsNoTracking()
            .Select(f => new FacultyDto(
                f.Id,
                f.Name,
                f.Code,
                f.Majors.Count
            ))
            .ToListAsync(cancellationToken);
    }
}