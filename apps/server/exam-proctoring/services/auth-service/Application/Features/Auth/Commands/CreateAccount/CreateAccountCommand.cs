using MediatR;
using SharedKernel.Core.Enums;

namespace auth_service.Application.Features.Auth.Commands.CreateAccount
{
    public record CreateAccountCommand(
        string Email,
        string Password,
        UserRole UserRole
    ) : IRequest<Guid>;
}