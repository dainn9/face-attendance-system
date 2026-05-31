using MediatR;

namespace user_service.Application.Features.Majors.Commands
{
    public record CreateMajorCommand(Guid FacultyId, string Name, string Code) : IRequest;
}