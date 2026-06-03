namespace attendance_service.API.Contracts.Subjects
{
    public record UpdateSubjectRequest
    (
        string Name,
        string Code,
        int Credits
    );
}