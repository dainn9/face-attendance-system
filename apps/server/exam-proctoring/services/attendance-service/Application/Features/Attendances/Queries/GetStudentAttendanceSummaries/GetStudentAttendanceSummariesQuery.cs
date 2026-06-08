using attendance_service.Application.Contracts.AttendanceSession;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetStudentAttendanceSummaries
{
    public record GetStudentAttendanceSummariesQuery(
        Guid CourseSectionId,
        IEnumerable<Guid> StudentIds
    ) : IRequest<Dictionary<Guid, StudentAttendanceSummaryDto>>;
}