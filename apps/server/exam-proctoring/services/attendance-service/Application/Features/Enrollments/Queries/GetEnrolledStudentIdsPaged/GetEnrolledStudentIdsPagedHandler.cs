using attendance_service.Application.Abstractions.Persistence;
using BuildingBlocks.Results;
using MediatR;

namespace attendance_service.Application.Features.Enrollments.Queries.GetEnrolledStudentIdsPaged
{
    public class GetEnrolledStudentIdsPagedHandler : IRequestHandler<GetEnrolledStudentIdsPagedQuery, PagedResult<Guid>>
    {
        private readonly ICourseSectionReadRepository _courseSectionReadRepository;

        public GetEnrolledStudentIdsPagedHandler(ICourseSectionReadRepository courseSectionReadRepository) =>
            _courseSectionReadRepository = courseSectionReadRepository;

        public Task<PagedResult<Guid>> Handle(GetEnrolledStudentIdsPagedQuery request, CancellationToken cancellationToken)
        => _courseSectionReadRepository.GetEnrolledStudentIdsPagedAsync(
            request.CourseSectionId,
            request.Page,
            request.PageSize,
            cancellationToken
        );
    }
}