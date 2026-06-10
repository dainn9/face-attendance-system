namespace user_service.API.Contracts.Majors
{
    public record UpdateMajorRequest(
        string Name,
        string Code
    );
}