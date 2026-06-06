namespace user_service.Application.Contracts.Students
{
    public record StudentLookupDto(
        Guid UserId,
        string UserCode,
        string FullName,
        string Email
    );
}