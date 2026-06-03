namespace attendance_service.API.Contracts.Subjects
{
    public record CreateSubjectRequest(
        string Name,
        string Code,
        int Credits
    );
}