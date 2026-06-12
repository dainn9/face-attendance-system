using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Queries.GetStudentActiveCourseSections
{
    public class GetStudentActiveCourseSectionsHandler : IRequestHandler<GetStudentActiveCourseSectionsQuery, IReadOnlyList<StudentCourseSectionDto>>
    {
        private readonly ICourseSectionReadRepository _courseSectionReadRepository;

        public GetStudentActiveCourseSectionsHandler(ICourseSectionReadRepository courseSectionReadRepository)
        {
            _courseSectionReadRepository = courseSectionReadRepository;
        }

        public Task<IReadOnlyList<StudentCourseSectionDto>> Handle(GetStudentActiveCourseSectionsQuery request, CancellationToken cancellationToken)
        => _courseSectionReadRepository.GetStudentActiveCourseSectionsAsync(request.StudentId, cancellationToken);
    }
}