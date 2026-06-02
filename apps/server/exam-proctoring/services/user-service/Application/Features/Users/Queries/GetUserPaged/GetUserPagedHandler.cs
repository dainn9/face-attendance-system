using BuildingBlocks.Results;
using MediatR;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts;

namespace user_service.Application.Features.Users.Queries.GetUserPaged
{
    public class GetUserPagedHandler : IRequestHandler<GetUserPagedQuery, PagedResult<UserPagedDto>>
    {
        private readonly IUserReadRepository _userReadRepository;

        public GetUserPagedHandler(IUserReadRepository userReadRepository)
        {
            _userReadRepository = userReadRepository;
        }

        public Task<PagedResult<UserPagedDto>> Handle(GetUserPagedQuery request, CancellationToken cancellationToken)
        => _userReadRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.SearchQuery,
            request.Role,
            request.FacultyId,
            cancellationToken
        );
    }
}