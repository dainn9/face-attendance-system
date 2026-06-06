namespace user_service.Application.Contracts.Users
{
    public record UserLookupDto(
        Guid UserId,
        string FullName
    );
}