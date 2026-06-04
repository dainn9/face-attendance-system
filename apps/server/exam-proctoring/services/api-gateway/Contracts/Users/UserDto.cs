namespace api_gateway.Contracts.Users
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

    public record UserLookupDto(
        Guid UserId,
        string FullName
    );
}