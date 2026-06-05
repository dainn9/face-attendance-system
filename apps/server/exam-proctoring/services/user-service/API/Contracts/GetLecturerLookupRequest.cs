namespace user_service.API.Contracts
{
    public sealed record GetLecturerLookupRequest(
        Guid? FacultyId,
        string? Keyword
    );
}