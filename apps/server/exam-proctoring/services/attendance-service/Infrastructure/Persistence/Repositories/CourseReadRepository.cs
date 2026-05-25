using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using BuildingBlocks.Results;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.Persistence.Repositories
{
    public class CourseReadRepository : ICourseReadRepository
    {
        private readonly AttendanceDbContext _context;
        private const int DefaultPageSize = 20;

        public CourseReadRepository(AttendanceDbContext Context) => _context = Context;

        public async Task<CourseDto?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken)
        => await _context.Courses
            .Where(c => c.Id == courseId)
            .Select(c => new CourseDto(
                c.Id,
                c.Name,
                c.Code,
                c.Credits
            ))
            .FirstOrDefaultAsync(cancellationToken);

        public async Task<PagedResult<CourseDto>> GetPagedAsync(int page, CancellationToken cancellationToken)
        {
            var items = await _context.Courses
                .AsNoTracking()
                .OrderBy(c => c.CreatedAt)
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .Select(c => new CourseDto(
                    c.Id,
                    c.Name,
                    c.Code,
                    c.Credits
                ))
                .ToListAsync(cancellationToken);

            var totalCount = await _context.Courses.CountAsync(cancellationToken);

            return new PagedResult<CourseDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = DefaultPageSize
            };
        }
    }
}