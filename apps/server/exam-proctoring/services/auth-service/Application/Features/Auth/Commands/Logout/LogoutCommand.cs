using auth_service.Domain.Enum;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand(Guid UserId, SessionType SessionType) : IRequest<Task>;
}