using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Domain.Aggregates.AttendanceSession;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.Persistence.Repositories
{
    public class AttendanceSessionRepository : IAttendanceSessionRepository
    {
        private readonly AttendanceDbContext _context;
        public AttendanceSessionRepository(AttendanceDbContext Context) => _context = Context;

        public void Add(AttendanceSession attendanceSession)
        => _context.AttendanceSessions.Add(attendanceSession);

        public async Task<AttendanceSession?> GetByIdWithRecordsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.AttendanceSessions
            .Include(s => s.Records)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        public async Task<IReadOnlyList<AttendanceSession>> GetByIdsWithRecordsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken = default)
        => await _context.AttendanceSessions
            .Include(s => s.Records)
            .Where(s => ids.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }
}