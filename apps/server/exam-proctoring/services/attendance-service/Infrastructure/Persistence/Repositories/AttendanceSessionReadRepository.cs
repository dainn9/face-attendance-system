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

            var presentCounts = await _context.AttendanceSessions
                .Where(s => s.CourseSectionId == courseSectionId && s.Status == AttendanceSessionStatus.Closed)
                .SelectMany(s => s.Records)
                .Where(r => ids.Contains(r.StudentId) && r.Status == AttendanceRecordStatus.Present)
                .GroupBy(r => r.StudentId)
                .Select(g => new
                {
                    StudentId = g.Key,
                    PresentSessions = g.Count()
                })
                .ToDictionaryAsync(x => x.StudentId, x => x.PresentSessions, cancellationToken);

            return ids.ToDictionary(
                studentId => studentId,
                studentId =>
                {
                    var present = presentCounts.GetValueOrDefault(studentId);

                    return new StudentAttendanceSummaryDto(
                        studentId,
                        present,
                        totalSessions,
                        totalSessions > 0
                            ? Math.Round(present * 100.0 / totalSessions, 2)
                            : 0
                    );
                });
        }

        public async Task<bool> HasOpenSessionAsync(Guid courseSectionId, CancellationToken cancellationToken = default)
        => await _context.AttendanceSessions
            .AnyAsync(s => s.CourseSectionId == courseSectionId && s.Status == AttendanceSessionStatus.Open, cancellationToken);
    }
}