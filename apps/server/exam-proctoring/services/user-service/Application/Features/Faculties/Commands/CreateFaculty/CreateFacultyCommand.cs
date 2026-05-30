using MediatR;

namespace user_service.Application.Features.Faculties.Commands.CreateFaculty
{
    public record CreateFacultyCommand(
        string Name,
        string Code
    ) : IRequest<Guid>;
}