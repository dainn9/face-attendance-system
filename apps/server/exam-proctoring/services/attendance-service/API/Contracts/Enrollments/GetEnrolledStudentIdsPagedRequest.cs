namespace attendance_service.API.Contracts.Enrollments
{
    public record GetEnrolledStudentIdsPagedRequest(
        int Page = 1,
        int PageSize = 10
    );
}