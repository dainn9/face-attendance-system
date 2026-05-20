using auth_service.Domain.Enum;

namespace auth_service.API.Contracts
{
    public record RegisterRequest(string Email, string Password, UserRole UserRole);
}