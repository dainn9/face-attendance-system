using SharedKernel.Core.Enums;

namespace api_gateway.Contracts
{
    public record CreateUserRequest(
        Guid UserId,
        string UserCode,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        string Email,
        UserRole Role,
        Guid? FacultyId,
        Guid? MajorId
    );
}