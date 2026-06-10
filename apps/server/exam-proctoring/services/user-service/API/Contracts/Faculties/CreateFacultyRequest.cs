namespace user_service.API.Contracts.Faculties
{
    public record CreateFacultyRequest(
        string Name,
        string Code
    );
}