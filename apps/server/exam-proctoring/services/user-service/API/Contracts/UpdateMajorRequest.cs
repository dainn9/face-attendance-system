namespace user_service.API.Contracts
{
    public record UpdateMajorRequest(
        string Name,
        string Code
    );
}