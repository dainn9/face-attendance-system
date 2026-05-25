namespace attendance_service.API.Contracts
{
    public record UpdateCourseRequest
    (
        string Name,
        string Code,
        int Credits
    );
}