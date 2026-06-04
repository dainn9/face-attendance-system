using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.Persistence.Repositories
{
    public class SubjectReadRepository : ISubjectReadRepository
    {
        private readonly AttendanceDbContext _context;

        public SubjectReadRepository(AttendanceDbContext Context) => _context = Context;

        public async Task<SubjectDto?> GetByIdAsync(Guid subjectId, CancellationToken cancellationToken)
        => await _context.Subjects
            .Where(s => s.Id == subjectId)
            .Select(s => new SubjectDto(
                s.Id,
                s.Name,
                s.Code,
                s.Credits
            ))
            .FirstOrDefaultAsync(cancellationToken);

        public async Task<IReadOnlyList<SubjectLookupDto>> GetSubjectLookupAsync(string? keyword, CancellationToken cancellationToken)
        {
            var query = _context.Subjects.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(s => s.Name.Contains(keyword) || s.Code.Contains(keyword));
            }

            return await query
                .OrderBy(x => x.Name)
                .Take(20)
                .Select(x => new SubjectLookupDto(
                    x.Id,
                     $"{x.Code} - {x.Name} ({x.Credits} TC)"
                ))
                .ToListAsync(cancellationToken);
        }

        // public async Task<PagedResult<SubjectDto>> GetPagedAsync(int page, CancellationToken cancellationToken)
        // {
        //     var items = await _context.Subjects
        //         .AsNoTracking()
        //         .OrderBy(s => s.CreatedAt)
        //         .Skip((page - 1) * DefaultPageSize)
        //         .Take(DefaultPageSize)
        //         .Select(s => new SubjectDto(
        //             s.Id,
        //             s.Name,
        //             s.Code,
        //             s.Credits
        //         ))
        //         .ToListAsync(cancellationToken);

        //     var totalCount = await _context.Subjects.CountAsync(cancellationToken);

        //     return new PagedResult<SubjectDto>
        //     {
        //         Items = items,
        //         TotalCount = totalCount,
        //         Page = page,
        //         PageSize = DefaultPageSize
        //     };
    }
}