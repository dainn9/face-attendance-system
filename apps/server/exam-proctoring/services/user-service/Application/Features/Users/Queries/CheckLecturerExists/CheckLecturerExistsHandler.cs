using MediatR;
using user_service.Application.Abstractions.Persistence;

namespace user_service.Application.Features.Users.Queries.CheckLecturerExists
{
    public class CheckLecturerExistsHandler : IRequestHandler<CheckLecturerExistsQuery, bool>
    {
        private readonly IUserReadRepository _userReadRepository;

        public CheckLecturerExistsHandler(IUserReadRepository userReadRepository) =>
            _userReadRepository = userReadRepository;

        public Task<bool> Handle(CheckLecturerExistsQuery request, CancellationToken cancellationToken)
        => _userReadRepository.CheckLecturerExistsByIdAsync(request.LecturerId, cancellationToken);
    }
}