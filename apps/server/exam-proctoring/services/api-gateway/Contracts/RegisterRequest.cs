using SharedKernel.Core.Enums;

namespace api_gateway.Contracts
{
    public record RegisterRequest(
        string Email,
        string Password,
        UserRole UserRole,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        string? StudentCode,
        string? ClassCode,
        string? FacultyCode
    );
}