using auth_service.Application.Contracts;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.LoginProfile
{
    public record LoginProfileCommand(string Email, string Password) : IRequest<AuthResponse>;
}