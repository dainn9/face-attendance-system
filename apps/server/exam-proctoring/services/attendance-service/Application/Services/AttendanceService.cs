using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Abstractions.Services;
using attendance_service.Domain.Enums;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Time;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceSessionReadRepository _attendanceSessionReadRepository;
        private readonly IAttendanceSessionRepository _attendanceSessionRepository;
        private readonly ICourseSectionReadRepository _courseSectionReadRepository;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AttendanceService> _logger;

        public AttendanceService(
            IAttendanceSessionReadRepository attendanceSessionReadRepository,
            IAttendanceSessionRepository attendanceSessionRepository,
            ICourseSectionReadRepository courseSectionReadRepository,
            IClock clock,
            IUnitOfWork unitOfWork,
            ILogger<AttendanceService> logger
            )
        {
            _attendanceSessionReadRepository = attendanceSessionReadRepository;
            _attendanceSessionRepository = attendanceSessionRepository;
            _courseSectionReadRepository = courseSectionReadRepository;
            _clock = clock;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CloseExpiredSessionsAsync(CancellationToken cancellationToken = default)
        {
            var sessionIds = await _attendanceSessionReadRepository
                .GetExpiredOpenAttendanceSessionIdsAsync(cancellationToken);

            if (sessionIds.Count == 0) return;

            var sessions = await _attendanceSessionRepository
                .GetByIdsWithRecordsAsync(sessionIds, cancellationToken);

            var courseSectionIds = sessions.Select(s => s.CourseSectionId).Distinct();

            var enrollmentMap = await _courseSectionReadRepository
                .GetEnrollmentStudentIdsAsync(courseSectionIds, cancellationToken);

            var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(_clock.UtcNow, TimeZoneConstants.VietnamTimeZoneId);

            foreach (var session in sessions)
            {
                if (session.Status == AttendanceSessionStatus.Closed)
                    continue;

                var studentIds = enrollmentMap.GetValueOrDefault(session.CourseSectionId, new HashSet<Guid>());

                session.Close(now);
                session.MarkAbsentStudents(studentIds, now);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogInformation("Some sessions in this batch were already closed by another process.");
            }
        }
    }
}