using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using BuildingBlocks.Results;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Queries.GetCourseSectionPaged
{
    public class GetCourseSectionPagedHandler : IRequestHandler<GetCourseSectionPagedQuery, PagedResult<CourseSectionPagedDto>>
    {
        private readonly ICourseSectionReadRepository _courseSectionReadRepository;

        public GetCourseSectionPagedHandler(ICourseSectionReadRepository courseSectionReadRepository) =>
            _courseSectionReadRepository = courseSectionReadRepository;

        public Task<PagedResult<CourseSectionPagedDto>> Handle(GetCourseSectionPagedQuery request, CancellationToken cancellationToken)
        => _courseSectionReadRepository.GetCourseSectionsPagedAsync(
            request.Page,
            request.PageSize,
            request.SearchQuery,
            request.FacultyId,
            request.Semester,
            request.AcademicYear,
            request.IsActive,
            cancellationToken
        );
    }
}