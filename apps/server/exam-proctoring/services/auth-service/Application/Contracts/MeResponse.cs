namespace auth_service.Application.Contracts
{
    public record MeResponse(
        Guid Id,
        string Email,
        string Role,
        bool IsActive
    );
}