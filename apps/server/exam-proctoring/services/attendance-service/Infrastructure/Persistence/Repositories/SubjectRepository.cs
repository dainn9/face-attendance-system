using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Domain.Aggregates.Subject;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.Persistence.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AttendanceDbContext _context;

        public SubjectRepository(AttendanceDbContext context) => _context = context;

        public void Add(Subject subject)
        => _context.Subjects.Add(subject);

        public void Update(Subject subject)
        => _context.Subjects.Update(subject);

        public async Task<Subject?> FindAsync(Guid subjectId, CancellationToken cancellationToken)
        => await _context.Subjects.FindAsync(subjectId, cancellationToken);

        public Task<bool> ExistsByCodeAsync(string code, Guid? excludeSubjectId, CancellationToken cancellationToken)
        => _context.Subjects.AsNoTracking().AnyAsync(s => s.Code == code && s.Id != excludeSubjectId, cancellationToken);
    }
}