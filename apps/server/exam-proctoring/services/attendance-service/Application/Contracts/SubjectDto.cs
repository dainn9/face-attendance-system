namespace attendance_service.Application.Contracts
{
    public record SubjectDto(
        Guid Id,
        string Name,
        string Code,
        int Credits
    );
}