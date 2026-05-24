using SharedKernel.Core.Enums;

namespace auth_service.Application.Contracts
{
    public record CreateUserRequest(
        Guid UserId,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        string Email,
        UserRole Role,
        string? StudentCode,
        string? LecturerCode,
        string? FacultyCode,
        string? MajorCode
    );
}