using attendance_service.Application.Contracts.AttendanceSession;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetAttendanceCheckInInfo
{
    public record GetAttendanceCheckInInfoQuery(Guid AttendanceSessionId) : IRequest<AttendanceCheckInInfoDto>;
}