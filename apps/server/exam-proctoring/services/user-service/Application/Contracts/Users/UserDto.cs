// using SharedKernel.Core.Enums;

using SharedKernel.Core.Enums;

namespace user_service.Application.Contracts.Users
{

    public sealed record UserDto(
        Guid UserId,
        string UserCode,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        string Email,
        UserRole Role
    );
}