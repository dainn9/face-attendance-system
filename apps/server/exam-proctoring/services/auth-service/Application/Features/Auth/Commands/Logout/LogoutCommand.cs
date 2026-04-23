using MediatR;

namespace auth_service.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand(string RefreshToken) : IRequest<Task>;
}