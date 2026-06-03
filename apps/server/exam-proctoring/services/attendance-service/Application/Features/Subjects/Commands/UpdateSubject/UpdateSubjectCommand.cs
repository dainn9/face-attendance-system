using MediatR;

namespace attendance_service.Application.Features.Subjects.Commands.UpdateSubject
{
    public record UpdateSubjectCommand(
        Guid SubjectId,
        string Name,
        string Code,
        int Credits
    ) : IRequest;
}