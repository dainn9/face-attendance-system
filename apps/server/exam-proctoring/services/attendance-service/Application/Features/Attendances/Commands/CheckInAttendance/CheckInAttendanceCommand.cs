using MediatR;

namespace attendance_service.Application.Features.Attendances.Commands.CheckInAttendance
{
    public record CheckInAttendanceCommand(
        Guid AttendanceSessionId,
        Guid StudentId,
        double Confidence
    ) : IRequest;
}