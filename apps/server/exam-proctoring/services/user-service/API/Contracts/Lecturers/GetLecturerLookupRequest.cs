namespace user_service.API.Contracts.Lecturers
{
    public sealed record GetLecturerLookupRequest(
        Guid? FacultyId,
        string? Keyword
    );
}