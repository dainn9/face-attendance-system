using Microsoft.EntityFrameworkCore;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts.Students;

namespace user_service.Infrastructure.Persistence.Repositories
{
    public class StudentReadRepository : IStudentReadRepository
    {
        private readonly UserDbContext _context;

        public StudentReadRepository(UserDbContext Context) => _context = Context;

        public async Task<Dictionary<Guid, StudentSummaryDto>> GetStudentSummariesByIdsAsync(IEnumerable<Guid> studentIds, CancellationToken cancellationToken)
        {
            var ids = studentIds.Distinct().ToList();

            if (ids.Count == 0)
                return new Dictionary<Guid, StudentSummaryDto>();

            var students = await _context.Users
                .AsNoTracking()
                .Where(u => ids.Contains(u.Id) && u.StudentProfile != null)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.UserCode,
                    MajorId = u.StudentProfile!.MajorId,
                    u.Email
                })
                .ToListAsync(cancellationToken);

            var majorIds = students
                .Select(s => s.MajorId)
                .Distinct()
                .ToList();

            var majorFacultyNames = await _context.Faculties
                .AsNoTracking()
                .SelectMany(f => f.Majors
                    .Where(m => majorIds.Contains(m.Id))
                    .Select(m => new
                    {
                        MajorId = m.Id,
                        FacultyName = f.Name
                    }))
                .ToDictionaryAsync(x => x.MajorId, x => x.FacultyName, cancellationToken);

            return students.ToDictionary(
                s => s.Id,
                s => new StudentSummaryDto(
                    s.Id,
                    s.UserCode ?? "",
                    s.FullName,
                    majorFacultyNames.GetValueOrDefault(s.MajorId, "-"),
                    s.Email
                )
            );
        }

        public async Task<IReadOnlyList<StudentLookupDto>> GetStudentLookupAsync(string? keyword, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 3)
                return [];

            return await _context.Users
                .AsNoTracking()
                .Where(u =>
                    u.StudentProfile != null &&
                    (u.FullName.Contains(keyword) ||
                        u.Email.Contains(keyword) ||
                        u.UserCode!.Contains(keyword)))
                .Take(20)
                .Select(u => new StudentLookupDto(
                    u.Id,
                    u.FullName,
                    u.UserCode!,
                    u.Email
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Guid>> GetExistingStudentIdsAsync(IEnumerable<Guid> studentIds, CancellationToken cancellationToken)
        {
            var ids = studentIds.Distinct().ToList();

            if (ids.Count == 0)
                return [];

            return await _context.Users
                .AsNoTracking()
                .Where(u => ids.Contains(u.Id) && u.StudentProfile != null)
                .Select(u => u.Id)
                .ToListAsync(cancellationToken);
        }
    }
}