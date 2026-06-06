namespace api_gateway.Contracts.Enrollments
{
    public record EnrollStudentsRequest(
        IReadOnlyList<Guid> StudentIds
    );
}