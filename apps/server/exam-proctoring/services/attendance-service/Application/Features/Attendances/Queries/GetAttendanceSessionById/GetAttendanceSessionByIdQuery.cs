using attendance_service.Application.Contracts.AttendanceSession;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetAttendanceSessionById
{
    public record GetAttendanceSessionByIdQuery(
        Guid Id
    ) : IRequest<AttendanceSessionDetailDto>;
}