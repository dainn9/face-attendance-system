using MediatR;

namespace attendance_service.Application.Features.Subjects.Commands.CreateSubject
{
    public record CreateSubjectCommand(
        Guid? FacultyId,
        string Name,
        string Code,
        int Credits
    ) : IRequest<Guid>;
}