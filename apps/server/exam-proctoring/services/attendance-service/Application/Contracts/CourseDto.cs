namespace attendance_service.Application.Contracts
{
    public record CourseDto(
        Guid Id,
        string Name,
        string Code,
        int Credits
    );
}