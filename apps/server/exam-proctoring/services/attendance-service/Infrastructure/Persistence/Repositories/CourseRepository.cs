using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Domain.Aggregates.Course;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.Persistence.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AttendanceDbContext _context;

        public CourseRepository(AttendanceDbContext context) => _context = context;

        public void Add(Course course)
        => _context.Courses.Add(course);

        public void Update(Course course)
        => _context.Courses.Update(course);

        public async Task<Course?> FindAsync(Guid courseId, CancellationToken cancellationToken)
        => await _context.Courses.FindAsync(courseId, cancellationToken);

        public Task<bool> ExistsByCodeAsync(string code, Guid? excludeCourseId, CancellationToken cancellationToken)
        => _context.Courses.AsNoTracking().AnyAsync(c => c.Code == code && c.Id != excludeCourseId, cancellationToken);
    }
}