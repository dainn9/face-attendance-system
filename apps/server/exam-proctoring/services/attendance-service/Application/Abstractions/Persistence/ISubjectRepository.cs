using attendance_service.Domain.Aggregates.Subject;

namespace attendance_service.Application.Abstractions.Persistence
{
    public interface ISubjectRepository
    {
        void Add(Subject subject);
        void Update(Subject subject);
        Task<Subject?> FindAsync(Guid subjectId, CancellationToken cancellationToken);
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeSubjectId, CancellationToken cancellationToken);
    }
}