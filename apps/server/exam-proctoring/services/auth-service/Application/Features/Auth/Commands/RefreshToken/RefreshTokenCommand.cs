using auth_service.Application.Contracts;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;
}