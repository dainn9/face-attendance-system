using auth_service.Domain.Enum;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(string Email, string Password, UserRole UserRole) : IRequest;

}