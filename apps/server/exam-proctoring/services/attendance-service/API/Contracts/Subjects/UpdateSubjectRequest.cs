namespace attendance_service.API.Contracts.Subjects
{
    public record UpdateSubjectRequest
    (
        Guid? FacultyId,
        string Name,
        string Code,
        int Credits
    );
}