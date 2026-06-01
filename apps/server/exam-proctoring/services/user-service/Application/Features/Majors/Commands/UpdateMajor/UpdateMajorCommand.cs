using MediatR;

namespace user_service.Application.Features.Majors.Commands.UpdateMajor
{
    public record UpdateMajorCommand(
        Guid FacultyId,
        Guid MajorId,
        string Name,
        string Code
    ) : IRequest;
}