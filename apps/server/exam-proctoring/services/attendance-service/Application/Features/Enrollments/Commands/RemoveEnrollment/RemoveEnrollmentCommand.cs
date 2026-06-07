using MediatR;

namespace attendance_service.Application.Features.Enrollments.Commands.RemoveEnrollment
{
    public record RemoveEnrollmentCommand(
        Guid CourseSectionId,
        Guid StudentId
    ) : IRequest;
}
