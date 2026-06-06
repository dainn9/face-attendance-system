using attendance_service.Domain.Aggregates.CourseSection;

namespace attendance_service.Application.Abstractions.Persistence
{
    public interface ICourseSectionRepository
    {
        void Add(CourseSection courseSection);
        Task<CourseSection?> GetWithEnrollmentsByIdAsync(Guid courseSectionId, CancellationToken cancellationToken);
    }
}