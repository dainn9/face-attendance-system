namespace user_service.API.Contracts
{
    public record CreateFacultyRequest(
        string Name,
        string Code
    );
}