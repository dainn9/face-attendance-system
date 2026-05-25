using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using BuildingBlocks.Results;
using MediatR;

namespace attendance_service.Application.Features.Courses.Queries.GetPaged
{
    public class GetPagedHandler : IRequestHandler<GetPagedQuery, PagedResult<CourseDto>>
    {
        private readonly ICourseReadRepository _courseReadRepository;

        public GetPagedHandler(ICourseReadRepository courseReadRepository)
        {
            _courseReadRepository = courseReadRepository;
        }

        public Task<PagedResult<CourseDto>> Handle(GetPagedQuery request, CancellationToken cancellationToken)
        => _courseReadRepository.GetPagedAsync(request.Page, cancellationToken);
    }
}