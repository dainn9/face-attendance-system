using auth_service.Application.Contracts;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.LoginAdmin
{
    public record LoginAdminCommand(string Email, string Password) : IRequest<AuthResponse>;
}