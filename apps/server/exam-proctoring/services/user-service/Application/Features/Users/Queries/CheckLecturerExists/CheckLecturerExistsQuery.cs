using MediatR;

namespace user_service.Application.Features.Users.Queries.CheckLecturerExists
{
    public record CheckLecturerExistsQuery(
        Guid LecturerId
    ) : IRequest<bool>;
}