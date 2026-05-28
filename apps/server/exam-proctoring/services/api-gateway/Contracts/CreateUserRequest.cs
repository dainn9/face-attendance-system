using SharedKernel.Core.Enums;

namespace api_gateway.Contracts
{
    public record CreateUserRequest(
        Guid UserId,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        string Email,
        UserRole Role,

        string? StudentCode,
        string? ClassCode,
        string? FacultyCode
    );
}