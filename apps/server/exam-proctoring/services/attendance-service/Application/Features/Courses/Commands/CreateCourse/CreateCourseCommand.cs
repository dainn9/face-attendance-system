using MediatR;

namespace attendance_service.Application.Features.Courses.Commands.CreateCourse
{
    public record CreateCourseCommand(
        string Name,
        string Code,
        int Credits
    ) : IRequest<Guid>;
}