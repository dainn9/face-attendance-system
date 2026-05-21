using SharedKernel.Core.Enums;

namespace auth_service.Application.Contracts
{
    public record CreateUserProfileRequest(Guid UserId, string FullName, Gender Gender, DateOnly DateOfBirth, string Email);
}