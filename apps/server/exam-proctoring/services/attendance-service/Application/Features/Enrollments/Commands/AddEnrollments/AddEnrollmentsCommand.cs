using MediatR;

namespace attendance_service.Application.Features.Enrollments.Commands.AddEnrollments
{
    public record AddEnrollmentsCommand(
        Guid CourseSectionId,
        IEnumerable<Guid> StudentIds
    ) : IRequest;
}