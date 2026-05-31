using Microsoft.EntityFrameworkCore;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts;

namespace user_service.Infrastructure.Persistence.Repositories
{
    public class UserReadRepository : IUserReadRepository
    {
        private readonly UserDbContext _context;

        public UserReadRepository(UserDbContext Context) => _context = Context;

        // public Task<UserDto?> GetProfileByIdAsync(Guid userId, CancellationToken ct)
        // => _context.Users
        //     .AsNoTracking()
        //     .Where(u => u.Id == userId)
        //     .Select(u => new UserDto(
        //         u.Id,
        //         u.FullName,
        //         u.Gender,
        //         u.DateOfBirth,
        //         u.Email,
        //         u.Role,

        //         u.StudentProfile != null ? u.StudentProfile.StudentCode : null,

        //         u.StudentProfile != null ? u.StudentProfile.ClassCode : null,

        //         u.LecturerProfile != null ? u.LecturerProfile.FacultyCode : null
        //     ))
        //     .FirstOrDefaultAsync(cancellationToken);

        public Task<Dictionary<Guid, int>> GetStudentCountByMajorsAsync(CancellationToken ct = default)
        => _context.Users
            .AsNoTracking()
            .Where(u => u.StudentProfile != null)
            .GroupBy(u => u.StudentProfile!.MajorId)
            .Select(g => new
            {
                MajorId = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(
                x => x.MajorId,
                x => x.Count,
                ct);

        public Task<Dictionary<Guid, int>> GetLecturerCountByFacultyAsync(CancellationToken ct = default)
        => _context.Users
            .AsNoTracking()
            .Where(u => u.LecturerProfile != null)
            .GroupBy(u => u.LecturerProfile!.FacultyId)
            .Select(g => new
            {
                FacultyId = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(
                x => x.FacultyId,
                x => x.Count,
                ct);

        public async Task<Dictionary<Guid, int>> GetStudentCountByFacultyIdAsync(Guid facultyId, CancellationToken ct = default)
        {
            var majorIds = await _context.Faculties
                .AsNoTracking()
                .Where(f => f.Id == facultyId)
                .SelectMany(f => f.Majors.Select(m => m.Id))
                .ToListAsync(ct);

            return await _context.Users
                .AsNoTracking()
                .Where(u =>
                    u.StudentProfile != null &&
                    majorIds.Contains(u.StudentProfile.MajorId))
                .GroupBy(u => u.StudentProfile!.MajorId)
                .Select(g => new
                {
                    MajorId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(
                    x => x.MajorId,
                    x => x.Count,
                    ct);
        }

        public async Task<IReadOnlyList<LecturerDto>> GetLecturersByFacultyIdAsync(Guid facultyId, CancellationToken ct = default)
        => await _context.Users
                .AsNoTracking()
                .Where(u => u.LecturerProfile != null && u.LecturerProfile.FacultyId == facultyId)
                .Select(u => new LecturerDto(
                    u.Id,
                    u.UserCode,
                    u.FullName
                ))
                .ToListAsync(ct);
    }
}