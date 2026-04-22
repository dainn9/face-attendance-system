using auth_service.Application.Contracts;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
}