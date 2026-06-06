namespace api_gateway.Contracts.Attendance
{
    public record GetEnrolledStudentIdsPageRequest(
        int Page = 1,
        int PageSize = 10
    );
}