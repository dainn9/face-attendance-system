using attendance_service.Application.Contracts.AttendanceSession;

namespace attendance_service.Application.Abstractions.Persistence
{
    public interface IAttendanceSessionReadRepository
    {
        Task<Dictionary<Guid, StudentAttendanceSummaryDto>> GetStudentAttendanceSummariesAsync(
            Guid courseSectionId,
            IEnumerable<Guid> studentIds,
            CancellationToken cancellationToken = default);

        Task<bool> HasOpenSessionAsync(Guid courseSectionId, CancellationToken cancellationToken = default);
    }
}