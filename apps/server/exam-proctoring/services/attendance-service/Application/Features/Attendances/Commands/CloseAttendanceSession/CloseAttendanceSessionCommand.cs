using MediatR;

namespace attendance_service.Application.Features.Attendances.Commands.CloseAttendanceSession
{
    public record CloseAttendanceSessionCommand(
        Guid LecturerId,
        Guid AttendanceSessionId
    ) : IRequest;
}