using MediatR;

namespace user_service.Application.Features.Faculties.Commands.UpdateFaculty
{
    public record UpdateFacultyCommand(
        Guid FacultyId,
        string Name,
        string Code
    ) : IRequest;
}