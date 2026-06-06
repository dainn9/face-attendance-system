using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Domain.Aggregates.CourseSection;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.Persistence.Repositories
{
    public class CourseSectionRepository : ICourseSectionRepository
    {
        private readonly AttendanceDbContext _context;

        public CourseSectionRepository(AttendanceDbContext Context) =>
            _context = Context;
        public void Add(CourseSection courseSection)
        => _context.CourseSections.Add(courseSection);

        public Task<CourseSection?> GetWithEnrollmentsByIdAsync(Guid courseSectionId, CancellationToken cancellationToken)
        => _context.CourseSections
            .Include(cs => cs.Enrollments)
            .FirstOrDefaultAsync(cs => cs.Id == courseSectionId, cancellationToken);
    }
}