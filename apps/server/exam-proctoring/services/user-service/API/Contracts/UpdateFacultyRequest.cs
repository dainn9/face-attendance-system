namespace user_service.API.Contracts
{
    public record UpdateFacultyRequest(
        string Name,
        string Code
    );
}