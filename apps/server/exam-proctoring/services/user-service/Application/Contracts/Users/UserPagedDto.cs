namespace user_service.Application.Contracts.Users
{
    public record UserPagedDto(
        Guid UserId,
        string UserCode,
        string FullName,
        string Email,
        string RoleName,
        string FacultyName
    );
}