using attendance_service.Application.Contracts;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Queries.GetCourseSectionDetail
{
    public record GetCourseSectionDetailQuery(Guid CourseSectionId) : IRequest<CourseSectionDetailDto>;
}