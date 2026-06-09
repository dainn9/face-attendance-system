using MediatR;

namespace attendance_service.Application.Features.Attendances.Commands.CreateAttendanceSession
{
    public record CreateAttendanceSessionCommand(
        Guid LecturerId,
        Guid CourseSectionId
    ) : IRequest<Guid>;
}