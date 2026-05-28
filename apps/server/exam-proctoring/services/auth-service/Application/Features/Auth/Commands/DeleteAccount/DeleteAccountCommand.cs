using MediatR;

namespace auth_service.Application.Features.Auth.Commands.DeleteAccount
{
    public record DeleteAccountCommand(Guid UserId) : IRequest;
}