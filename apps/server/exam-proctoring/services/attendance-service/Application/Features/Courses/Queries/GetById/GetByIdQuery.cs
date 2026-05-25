using attendance_service.Application.Contracts;
using MediatR;

namespace attendance_service.Application.Features.Courses.Queries.GetById
{
    public record GetByIdQuery(Guid CourseId) : IRequest<CourseDto>;
}