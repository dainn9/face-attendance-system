using BuildingBlocks.Results;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Core.Enums;
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

        public async Task<PagedResult<UserPagedDto>> GetPagedAsync(
            int page,
            int pageSize,
            string? searchQuery = null,
            UserRole? role = null,
            Guid? facultyId = null,
            CancellationToken ct = default
        )
        {
            var query = _context.Users.AsNoTracking();
            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchQuery))
                query = query.Where(u =>
                    u.FullName.Contains(searchQuery) ||
                    u.Email.Contains(searchQuery) ||
                    u.UserCode.Contains(searchQuery)
                );

            // Apply role filter
            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            // Apply faculty filter
            if (facultyId.HasValue)
            {
                var facultyMajorIds = await _context.Faculties
                    .AsNoTracking()
                    .Where(f => f.Id == facultyId.Value)
                    .SelectMany(f => f.Majors.Select(m => m.Id))
                    .ToListAsync(ct);

                query = query.Where(u =>

                    // Lecturer filter
                    (u.LecturerProfile != null &&
                     u.LecturerProfile.FacultyId == facultyId.Value)

                    ||

                    // Student filter
                    (u.StudentProfile != null &&
                     facultyMajorIds.Contains(u.StudentProfile.MajorId))
                );
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync(ct);

            // Apply pagination and projection
            var projections = await query
                .OrderBy(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserPagedProjection(
                    u.Id,
                    u.Role == UserRole.Admin ? "-" : u.UserCode,
                    u.FullName,
                    u.Email,
                    u.Role.ToString(),
                    u.LecturerProfile != null ? u.LecturerProfile.FacultyId : null,
                    u.StudentProfile != null ? u.StudentProfile.MajorId : null
                ))
                .ToListAsync(ct);

            // Extract distinct faculty and major IDs for batch fetching related names
            var facultyIdsOfProjections = projections
                .Where(p => p.FacultyId.HasValue)
                .Select(p => p.FacultyId!.Value)
                .Distinct()
                .ToList();

            var majorIdsOfProjections = projections
                .Where(p => p.MajorId.HasValue)
                .Select(p => p.MajorId!.Value)
                .Distinct()
                .ToList();

            var facultyNames = facultyIdsOfProjections.Count == 0
                ? new Dictionary<Guid, string>()
                : await _context.Faculties
                        .AsNoTracking()
                        .Where(f => facultyIdsOfProjections.Contains(f.Id))
                        .ToDictionaryAsync(
                            f => f.Id,
                            f => f.Name,
                            ct
                        );

            var majorFacultyNames = majorIdsOfProjections.Count == 0
                ? new Dictionary<Guid, string>()
                : await _context.Faculties
                    .AsNoTracking()
                    .SelectMany(f => f.Majors
                            .Where(m => majorIdsOfProjections.Contains(m.Id))
                            .Select(m => new
                            {
                                MajorId = m.Id,
                                FacultyName = f.Name
                            }))
                    .ToDictionaryAsync(
                        x => x.MajorId,
                        x => x.FacultyName,
                        ct
                    );

            // Map projections to DTOs with related names
            var items = projections.Select(p => new UserPagedDto(
                p.Id,
                p.UserCode,
                p.FullName,
                p.Email,
                p.Role,
                p.FacultyId.HasValue ? facultyNames.GetValueOrDefault(p.FacultyId.Value, "-")
                : (p.MajorId.HasValue ? majorFacultyNames.GetValueOrDefault(p.MajorId.Value, "-") : "-")

            )).ToList();

            return new PagedResult<UserPagedDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Dictionary<Guid, UserLookupDto>> GetLecturersByIdsAsync(
            IEnumerable<Guid> userIds,
            CancellationToken cancellationToken)
        {
            var ids = userIds.Distinct().ToList();

            return await _context.Users
                .AsNoTracking()
                .Where(u => ids.Contains(u.Id) && u.LecturerProfile != null)
                .Select(u => new UserLookupDto(
                    u.Id,
                    u.FullName
                ))
                .ToDictionaryAsync(u => u.UserId, cancellationToken);
        }

        public async Task<IReadOnlyList<UserLookupDto>> GetLecturerLookupByFacultyIdAsync(Guid? facultyId, string? keyWord, CancellationToken cancellationToken)
        {
            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.LecturerProfile != null);

            if (facultyId.HasValue)
                query = query.Where(u => u.LecturerProfile!.FacultyId == facultyId.Value);

            if (!string.IsNullOrWhiteSpace(keyWord))
                query = query.Where(u => u.FullName.Contains(keyWord) || u.UserCode.Contains(keyWord));

            return await query
            .OrderBy(u => u.FullName)
            .Select(u => new UserLookupDto(
                u.Id,
                $"{u.FullName} ({u.UserCode})"
            )).ToListAsync(cancellationToken);
        }


        public record UserPagedProjection(
            Guid Id,
            string UserCode,
            string FullName,
            string Email,
            string Role,
            Guid? FacultyId,
            Guid? MajorId
        );
    }
}