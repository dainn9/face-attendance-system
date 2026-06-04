namespace attendance_service.API.Contracts.Subjects
{
    public record CreateSubjectRequest(
        Guid? FacultyId,
        string Name,
        string Code,
        int Credits
    );
}