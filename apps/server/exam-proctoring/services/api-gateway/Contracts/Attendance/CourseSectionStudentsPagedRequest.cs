namespace api_gateway.Contracts.Attendance
{
    public record CourseSectionStudentsPagedRequest(
        int Page = 1,
        int PageSize = 10
    );
}