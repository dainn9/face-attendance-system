using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using BuildingBlocks.Results;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Queries.GetCoureseSectionPagedByLecturerId
{
    public class GetLecturerCourseSectionsHandler : IRequestHandler<GetLecturerCourseSectionsQuery, PagedResult<LecturerCourseSectionDto>>
    {
        private readonly ICourseSectionReadRepository _courseSectionReadRepository;

        public GetLecturerCourseSectionsHandler(ICourseSectionReadRepository courseSectionReadRepository)
        {
            _courseSectionReadRepository = courseSectionReadRepository;
        }

        public Task<PagedResult<LecturerCourseSectionDto>> Handle(GetLecturerCourseSectionsQuery request, CancellationToken cancellationToken)
        => _courseSectionReadRepository.GetLecturerCourseSectionsAsync(
            lecturerId: request.LecturerId,
            page: request.Page,
            pageSize: request.PageSize,
            searchQuery: request.SearchQuery,
            semester: request.Semester,
            academicYear: request.AcademicYear,
            isActive: request.IsActive,
            cancellationToken: cancellationToken
        );
    }
}