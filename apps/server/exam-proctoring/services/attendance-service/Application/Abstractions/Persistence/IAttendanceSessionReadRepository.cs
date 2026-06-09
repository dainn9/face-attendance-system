using attendance_service.Application.Contracts.AttendanceSession;
using BuildingBlocks.Results;

namespace attendance_service.Application.Abstractions.Persistence
{
    public interface IAttendanceSessionReadRepository
    {
        Task<Dictionary<Guid, StudentAttendanceSummaryDto>> GetStudentAttendanceSummariesAsync(
            Guid courseSectionId,
            IEnumerable<Guid> studentIds,
            CancellationToken cancellationToken = default);

        Task<bool> HasOpenSessionAsync(Guid courseSectionId, CancellationToken cancellationToken = default);
        Task<PagedResult<AttendanceSessionHistoryDto>> GetAttendanceSessionHistoryAsync(
            Guid courseSectionId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default
        );
    }
}