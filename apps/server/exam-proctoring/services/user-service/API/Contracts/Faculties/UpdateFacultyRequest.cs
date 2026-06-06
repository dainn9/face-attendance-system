namespace user_service.API.Contracts.Faculties
{
    public record UpdateFacultyRequest(
        string Name,
        string Code
    );
}