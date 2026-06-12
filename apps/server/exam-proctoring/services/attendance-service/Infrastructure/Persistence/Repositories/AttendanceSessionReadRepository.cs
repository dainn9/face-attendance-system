using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts.AttendanceSession;
using attendance_service.Domain.Enums;
using BuildingBlocks.Exceptions;
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

            var totalStudents = await _context.CourseSections
               .AsNoTracking()
               .Where(cs => cs.Id == courseSectionId)
               .SelectMany(cs => cs.Enrollments)
               .CountAsync(cancellationToken);

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
                    s.Status == AttendanceSessionStatus.Closed
                        ? s.Records.Count(r => r.Status == AttendanceRecordStatus.Absent)
                        : totalStudents - s.Records.Count(r => r.Status == AttendanceRecordStatus.Present),
                    totalStudents == 0
                        ? 0.0
                        : Math.Round(s.Records.Count(r => r.Status == AttendanceRecordStatus.Present) * 100.0 / totalStudents, 2)
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
        {
            var session = await _context.AttendanceSessions
                .AsNoTracking()
                .Where(s => s.Id == attendanceSessionId)
                .Select(s => new
                {
                    s.Id,
                    s.CourseSectionId,
                    s.Date,
                    s.StartTime,
                    s.EndTime,
                    s.Status,
                    PresentCount = s.Records.Count(r => r.Status == AttendanceRecordStatus.Present),
                    AbsentCount = s.Records.Count(r => r.Status == AttendanceRecordStatus.Absent),
                })
                .FirstOrDefaultAsync(cancellationToken);


            if (session is null)
                throw new EntityNotFoundException("Attendance session", attendanceSessionId);

            var totalStudents = await _context.CourseSections
                .AsNoTracking()
                .Where(cs => cs.Id == session.CourseSectionId)
                .SelectMany(cs => cs.Enrollments)
                .CountAsync(cancellationToken);

            var absenceCount = session.Status == AttendanceSessionStatus.Closed
                ? session.AbsentCount
                : totalStudents - session.PresentCount;

            var rate = totalStudents > 0
                ? Math.Round(session.PresentCount * 100.0 / totalStudents, 2)
                : 0;
            return new AttendanceSessionDetailDto(
                session.Id,
                session.Date,
                session.StartTime,
                session.EndTime,
                session.Status,
                session.PresentCount,
                absenceCount,
                rate
            );
        }

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

        public async Task<AttendanceCheckInInfoDto?> GetAttendanceCheckInInfoAsync(
            Guid attendanceSessionId,
            CancellationToken cancellationToken = default)
            => await _context.AttendanceSessions
                .AsNoTracking()
                .Where(s => s.Id == attendanceSessionId)
                .Join(
                    _context.CourseSections.AsNoTracking(),
                    s => s.CourseSectionId,
                    cs => cs.Id,
                    (s, cs) => new { s, cs }
                )
                .Join(
                    _context.Subjects.AsNoTracking(),
                    x => x.cs.SubjectId,
                    subj => subj.Id,
                    (x, subj) => new AttendanceCheckInInfoDto(
                        x.s.Id,
                        subj.Name,
                        x.cs.CourseSectionCode,
                        x.s.Date,
                        x.s.StartTime,
                        x.s.Status
                    )
                )
                .FirstOrDefaultAsync(cancellationToken);
    }
}
