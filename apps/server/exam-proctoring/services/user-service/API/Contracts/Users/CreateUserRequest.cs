using SharedKernel.Core.Enums;

namespace user_service.API.Contracts.Users
{
    public record CreateUserRequest(
        Guid UserId,
        string? UserCode,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        string Email,
        UserRole Role,
        Guid? FacultyId,
        Guid? MajorId
    );
}