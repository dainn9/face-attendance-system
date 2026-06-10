using auth_service.Application.Abstractions.Persistence;
using MediatR;

namespace auth_service.Application.Features.Auth.Queries.GetStatusByIds
{
    public class GetStatusByIdsHandler : IRequestHandler<GetStatusByIdsQuery, Dictionary<Guid, bool>>
    {
        private readonly IUserRepository _userRepository;

        public GetStatusByIdsHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Dictionary<Guid, bool>> Handle(GetStatusByIdsQuery request, CancellationToken cancellationToken)
        => await _userRepository.GetStatusByIdsAsync(request.UserIds, cancellationToken);
    }
}