using MediatR;

namespace attendance_service.Application.Features.Subjects.Commands.CreateSubject
{
    public record CreateSubjectCommand(
        string Name,
        string Code,
        int Credits
    ) : IRequest<Guid>;
}