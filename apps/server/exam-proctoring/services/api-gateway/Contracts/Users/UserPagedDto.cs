namespace api_gateway.Contracts.Userss
{
    public record UserPagedDto(
        Guid UserId,
        string UserCode,
        string FullName,
        string Email,
        string RoleName,
        string FacultyName,
        bool? IsActive  // merge từ AuthService
    );
}