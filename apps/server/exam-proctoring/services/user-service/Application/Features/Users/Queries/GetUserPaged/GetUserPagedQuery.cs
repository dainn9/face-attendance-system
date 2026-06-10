using BuildingBlocks.Results;
using MediatR;
using SharedKernel.Core.Enums;
using user_service.Application.Contracts.Users;

namespace user_service.Application.Features.Users.Queries.GetUserPaged
{
    public record GetUserPagedQuery(
        int Page = 1,
        int PageSize = 10,
        string? SearchQuery = null,
        UserRole? Role = null,
        Guid? FacultyId = null
    ) : IRequest<PagedResult<UserPagedDto>>;
}