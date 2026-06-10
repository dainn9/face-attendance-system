using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Queries.GetLecturerCourseSectionLookup
{
    public class GetLecturerCourseSectionLookupHandler : IRequestHandler<GetLecturerCourseSectionLookupQuery, IReadOnlyList<LecturerCourseSectionLookupDto>>
    {
        private readonly ICourseSectionReadRepository _courseSectionReadRepository;

        public GetLecturerCourseSectionLookupHandler(ICourseSectionReadRepository courseSectionReadRepository)
        {
            _courseSectionReadRepository = courseSectionReadRepository;
        }

        public Task<IReadOnlyList<LecturerCourseSectionLookupDto>> Handle(GetLecturerCourseSectionLookupQuery request, CancellationToken cancellationToken)
        => _courseSectionReadRepository.GetLecturerCourseSectionLookupAsync(request.LecturerId, cancellationToken);
    }
}