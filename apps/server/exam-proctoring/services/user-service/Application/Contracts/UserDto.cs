// using SharedKernel.Core.Enums;

namespace user_service.Application.Contracts
{
    public record LecturerDto(
        Guid UserId,
        string UserCode,
        string FullName
    );

    public record UserPagedDto(
        Guid UserId,
        string UserCode,
        string FullName,
        string Email,
        string RoleName,
        string FacultyName
    );
    public record UserLookupDto(
        Guid UserId,
        string FullName
    );

    //     public sealed record UserDto(
    //         Guid UserId,
    //         string UserCode,
    //         string FullName,
    //         Gender Gender,
    //         DateOnly DateOfBirth,
    //         string Email,
    //         UserRole Role,

    //         string? StudentCode,
    //         string? CLassCode,
    //         string? FacultyCode
    //     );
}