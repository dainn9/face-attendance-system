using SharedKernel.Core.Enums;

namespace api_gateway.Contracts
{
    public record RegisterRequest(
        string? UserCode,
        string Email,
        string Password,
        UserRole UserRole,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        Guid? FacultyId,
        Guid? MajorId
    );
}