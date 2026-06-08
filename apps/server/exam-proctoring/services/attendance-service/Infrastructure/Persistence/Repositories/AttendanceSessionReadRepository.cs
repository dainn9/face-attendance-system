using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts.AttendanceSession;
using attendance_service.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.Persistence.Repositories
{
    public class AttendanceSessionReadRepository : IAttendanceSessionReadRepository
    {
        private readonly AttendanceDbContext _context;

        public AttendanceSessionReadRepository(AttendanceDbContext context)
        => _context = context;

        public async Task<Dictionary<Guid, StudentAttendanceSummaryDto>> GetStudentAttendanceSummariesAsync(
            Guid courseSectionId,
            IEnumerable<Guid> studentIds,
            CancellationToken cancellationToken = default)
        {
            var ids = studentIds.ToHashSet();

            if (ids.Count == 0)
                return new Dictionary<Guid, StudentAttendanceSummaryDto>();

            var totalSessions = await _context.AttendanceSessions.
                Where(s => s.CourseSectionId == courseSectionId && s.Status == AttendanceSessionStatus.Closed)
                .CountAsync(cancellationToken);

            return await _context.AttendanceSessions
                .Where(s => s.CourseSectionId == courseSectionId && s.Status == AttendanceSessionStatus.Closed)
                .SelectMany(s => s.Records)
                .Where(r => ids.Contains(r.StudentId) && r.Status == AttendanceRecordStatus.Present)
                .GroupBy(r => r.StudentId)
                .Select(g => new StudentAttendanceSummaryDto(
                    g.Key,
                    g.Count(),
                    totalSessions,
                    totalSessions > 0
                        ? Math.Round(g.Count() * 100.0 / totalSessions, 2)
                        : 0
                ))
                .ToDictionaryAsync(s => s.StudentId, cancellationToken);
        }
    }
}