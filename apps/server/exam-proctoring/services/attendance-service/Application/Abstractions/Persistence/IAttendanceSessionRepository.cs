using attendance_service.Domain.Aggregates.AttendanceSession;

namespace attendance_service.Application.Abstractions.Persistence
{
    public interface IAttendanceSessionRepository
    {
        void Add(AttendanceSession attendanceSession);
        Task<AttendanceSession?> GetByIdWithRecordsAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AttendanceSession>> GetByIdsWithRecordsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken = default);
    }
}