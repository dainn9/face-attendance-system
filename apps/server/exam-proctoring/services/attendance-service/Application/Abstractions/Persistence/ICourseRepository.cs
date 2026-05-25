using attendance_service.Domain.Aggregates.Course;

namespace attendance_service.Application.Abstractions.Persistence
{
    public interface ICourseRepository
    {
        void Add(Course course);
        void Update(Course course);
        Task<Course?> FindAsync(Guid courseId, CancellationToken cancellationToken);
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeCourseId, CancellationToken cancellationToken);
    }
}