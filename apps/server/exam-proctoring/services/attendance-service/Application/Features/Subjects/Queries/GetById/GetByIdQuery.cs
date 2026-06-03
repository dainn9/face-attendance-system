using attendance_service.Application.Contracts;
using MediatR;

namespace attendance_service.Application.Features.Subjects.Queries.GetById
{
    public record GetByIdQuery(Guid SubjectId) : IRequest<SubjectDto>;
}