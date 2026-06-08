using attendance_service.Application.Contracts;
using MediatR;
using SharedKernel.Core.Enums;

namespace attendance_service.Application.Features.CourseSections.Queries.GetCourseSectionDetail
{
    public record GetCourseSectionDetailQuery(Guid UserId, UserRole Role, Guid CourseSectionId) : IRequest<CourseSectionDetailDto>;
}