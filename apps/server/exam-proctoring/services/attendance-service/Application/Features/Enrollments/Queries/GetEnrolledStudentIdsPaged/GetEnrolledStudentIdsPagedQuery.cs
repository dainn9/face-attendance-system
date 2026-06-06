using BuildingBlocks.Results;
using MediatR;

namespace attendance_service.Application.Features.Enrollments.Queries.GetEnrolledStudentIdsPaged
{
    public record GetEnrolledStudentIdsPagedQuery(
        Guid CourseSectionId,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<PagedResult<Guid>>;
}