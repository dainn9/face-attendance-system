using MediatR;

namespace attendance_service.Application.Features.Subjects.Commands.UpdateSubject
{
    public record UpdateSubjectCommand(
        Guid SubjectId,
        Guid? FacultyId,
        string Name,
        string Code,
        int Credits
    ) : IRequest;
}