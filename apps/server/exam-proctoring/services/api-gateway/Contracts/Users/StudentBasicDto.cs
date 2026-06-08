namespace api_gateway.Contracts.Users
{
    public record StudentBasicDto(
        Guid UserId,
        string UserCode,
        string FullName,
        string Email
    );
}