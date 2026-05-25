using attendance_service.Application.Contracts;
using BuildingBlocks.Results;
using MediatR;

namespace attendance_service.Application.Features.Courses.Queries.GetPaged
{
    public record GetPagedQuery(int Page = 1) : IRequest<PagedResult<CourseDto>>;
}