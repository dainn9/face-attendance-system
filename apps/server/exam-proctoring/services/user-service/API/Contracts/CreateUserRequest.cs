using SharedKernel.Core.Enums;

namespace user_service.API.Contracts
{
    public record CreateUserRequest(Guid UserId, string FullName, Gender Gender, DateOnly DateOfBirth, string Email);
}