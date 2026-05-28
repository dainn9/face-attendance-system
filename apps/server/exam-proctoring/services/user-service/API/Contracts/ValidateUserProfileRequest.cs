using SharedKernel.Core.Enums;

namespace user_service.API.Contracts
{
    public record ValidateUserProfileRequest(
        UserRole Role,
        string? StudentCode,
        string? ClassCode,
        string? FacultyCode
    );
}