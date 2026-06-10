using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using BuildingBlocks.Exceptions;
using MediatR;
using SharedKernel.Core.Enums;

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
        {
            var courseSectionDetail = await _courseSectionReadRepository.GetCourseSectionDetailAsync(request.CourseSectionId, cancellationToken)
                ?? throw new EntityNotFoundException("CourseSection", request.CourseSectionId);

            switch (request.Role)
            {
                case UserRole.Admin:
                    break;

                case UserRole.Lecturer:
                    if (courseSectionDetail.LecturerId != request.UserId)
                        throw new ForbiddenException("You do not have permission to access this course section");
                    break;

                case UserRole.Student:
                    var isEnrolled = await _courseSectionReadRepository.IsStudentEnrolledAsync(
                        request.CourseSectionId,
                        request.UserId,
                        cancellationToken);

                    if (!isEnrolled)
                        throw new ForbiddenException("You do not have permission to access this course section");
                    break;
            }

            return courseSectionDetail;
        }
    }
}