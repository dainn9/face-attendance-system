using SharedKernel.Core.Enums;

namespace auth_service.API.Contracts
{
    public record CreateAccountRequest(
        string Email,
        string Password,
        UserRole UserRole
    );
}