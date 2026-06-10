using SharedKernel.Core.Enums;

namespace api_gateway.Contracts
{
    public record CreateAccountRequest(
        string Email,
        string Password,
        UserRole UserRole
    );
}