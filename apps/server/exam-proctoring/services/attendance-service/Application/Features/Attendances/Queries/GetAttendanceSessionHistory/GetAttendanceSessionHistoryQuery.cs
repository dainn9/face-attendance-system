using attendance_service.Application.Contracts.AttendanceSession;
using BuildingBlocks.Results;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetAttendanceSessionHistory
{
    public record GetAttendanceSessionHistoryQuery(
        Guid CourseSectionId,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<PagedResult<AttendanceSessionHistoryDto>>;
}