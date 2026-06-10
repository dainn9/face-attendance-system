using MediatR;
using user_service.Application.Contracts.Lecturers;

namespace user_service.Application.Features.Users.Queries.GetLecturerById
{
    public record GetLecturerByIdQuery(Guid Id) : IRequest<LecturerDto>;
}