using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts.AttendanceSession;
using attendance_service.Domain.Enums;
using BuildingBlocks.Results;
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

        public async Task<PagedResult<AttendanceSessionHistoryDto>> GetAttendanceSessionHistoryAsync(
            Guid courseSectionId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default
        )
        {
            var query = _context.AttendanceSessions
                .AsNoTracking()
                .Where(s => s.CourseSectionId == courseSectionId)
                .OrderByDescending(s => s.Date)
                .ThenByDescending(s => s.StartTime);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new AttendanceSessionHistoryDto(
                    s.Id,
                    s.Date,
                    s.StartTime,
                    s.EndTime,
                    s.Status,
                    s.Records.Count(r => r.Status == AttendanceRecordStatus.Present),
                    s.Records.Count(r => r.Status == AttendanceRecordStatus.Absent),
                    s.Records.Count == 0
                        ? 0.0
                        : s.Records.Count(r => r.Status == AttendanceRecordStatus.Present) * 100.0 / s.Records.Count
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<AttendanceSessionHistoryDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<AttendanceSessionDetailDto?> GetAttendanceSessionByIdAsync(
            Guid attendanceSessionId,
            CancellationToken cancellationToken = default
        )
        => await _context.AttendanceSessions
            .AsNoTracking()
            .Where(s => s.Id == attendanceSessionId)
            .Select(s => new AttendanceSessionDetailDto(
                s.Id,
                s.Date,
                s.StartTime,
                s.EndTime,
                s.Status,
                s.Records.Count(r => r.Status == AttendanceRecordStatus.Present),
                s.Records.Count(r => r.Status == AttendanceRecordStatus.Absent),
                s.Records.Count == 0
                    ? 0.0
                    : s.Records.Count(r => r.Status == AttendanceRecordStatus.Present) * 100.0 / s.Records.Count

            ))
            .FirstOrDefaultAsync(cancellationToken);

        public async Task<Dictionary<Guid, AttendanceRecordDto>> GetAttendanceRecordsBySessionIdAsync(Guid attendanceSessionId, CancellationToken cancellationToken = default)
        => await _context.AttendanceSessions
            .AsNoTracking()
            .Where(r => r.Id == attendanceSessionId)
            .SelectMany(s => s.Records)
            .Select(r => new AttendanceRecordDto(
                r.StudentId,
                r.Status,
                r.Confidence,
                r.CheckedInAt
            ))
            .ToDictionaryAsync(r => r.StudentId, cancellationToken: cancellationToken);
    }
}
