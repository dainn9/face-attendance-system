namespace user_service.Application.Contracts.Students
{
    public record StudentBasicDto(
        Guid UserId,
        string UserCode,
        string FullName,
        string Email
    );
}