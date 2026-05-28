using SharedKernel.Core.Enums;

namespace user_service.Application.Contracts
{
    public sealed record UserDto(
        Guid UserId,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        string Email,
        UserRole Role,

        string? StudentCode,
        string? CLassCode,
        string? FacultyCode
    );
}