using MediatR;

namespace attendance_service.Application.Features.Courses.Commands.UpdateCourse
{
    public record UpdateCourseCommand(
        Guid CourseId,
        string Name,
        string Code,
        int Credits
    ) : IRequest;
}