namespace attendance_service.API.Contracts
{
    public record CreateCourseRequest(
        string Name,
        string Code,
        int Credits
    );
}