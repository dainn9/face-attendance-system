using user_service.Domain.Enum;

namespace user_service.API.Contracts
{
    public record CreateUserRequest(Guid UserId, string FullName, Gender Gender, DateOnly DateOfBirth, string Email);
}