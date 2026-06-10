using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts.Users;

namespace user_service.Application.Features.Users.Queries.GetLecturersByIds
{
    public class GetLecturersByIdsHandler : IRequestHandler<GetLecturersByIdsQuery, Dictionary<Guid, UserLookupDto>>
    {
        private readonly IUserReadRepository _userReadRepository;

        public GetLecturersByIdsHandler(IUserReadRepository userReadRepository) =>
            _userReadRepository = userReadRepository;

        public Task<Dictionary<Guid, UserLookupDto>> Handle(GetLecturersByIdsQuery request, CancellationToken cancellationToken)
        => _userReadRepository.GetLecturersByIdsAsync(request.UserIds, cancellationToken);
    }
}