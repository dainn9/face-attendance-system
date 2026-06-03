using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using BuildingBlocks.Results;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.Persistence.Repositories
{
    public class SubjectReadRepository : ISubjectReadRepository
    {
        private readonly AttendanceDbContext _context;
        private const int DefaultPageSize = 20;

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

        public async Task<PagedResult<SubjectDto>> GetPagedAsync(int page, CancellationToken cancellationToken)
        {
            var items = await _context.Subjects
                .AsNoTracking()
                .OrderBy(s => s.CreatedAt)
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .Select(s => new SubjectDto(
                    s.Id,
                    s.Name,
                    s.Code,
                    s.Credits
                ))
                .ToListAsync(cancellationToken);

            var totalCount = await _context.Subjects.CountAsync(cancellationToken);

            return new PagedResult<SubjectDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = DefaultPageSize
            };
        }
    }
}