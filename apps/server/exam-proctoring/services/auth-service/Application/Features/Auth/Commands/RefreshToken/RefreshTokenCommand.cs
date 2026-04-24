using auth_service.Application.Contracts;
using auth_service.Domain.Enum;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(Guid UserId, string RefreshToken, SessionType SessionType) : IRequest<AuthResponse>;
}