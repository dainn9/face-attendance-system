using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using BuildingBlocks.Exceptions;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Queries.GetCourseSectionDetail
{
    public class GetCourseSectionDetailHandler : IRequestHandler<GetCourseSectionDetailQuery, CourseSectionDetailDto>
    {
        private readonly ICourseSectionReadRepository _courseSectionReadRepository;

        public GetCourseSectionDetailHandler(ICourseSectionReadRepository courseSectionReadRepository)
        {
            _courseSectionReadRepository = courseSectionReadRepository;
        }

        public async Task<CourseSectionDetailDto> Handle(GetCourseSectionDetailQuery request, CancellationToken cancellationToken)
        => await _courseSectionReadRepository.GetCourseSectionDetailAsync(request.CourseSectionId, cancellationToken)
        ?? throw new EntityNotFoundException("CourseSection", request.CourseSectionId);
    }
}