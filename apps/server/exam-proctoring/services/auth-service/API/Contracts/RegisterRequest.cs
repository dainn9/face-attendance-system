using SharedKernel.Core.Enums;

namespace auth_service.API.Contracts
{
    public record RegisterRequest(
        string Email,
        string Password,
        UserRole UserRole,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        string? StudentCode,
        string? LecturerCode,
        string? FacultyCode,
        string? MajorCode
    );
}