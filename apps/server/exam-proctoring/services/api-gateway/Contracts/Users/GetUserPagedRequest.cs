using SharedKernel.Core.Enums;

namespace api_gateway.Contracts.Users
{
    public record GetUserPagedRequest(
        int Page = 1,
        int PageSize = 10,
        string? SearchQuery = null,
        UserRole? Role = null,
        Guid? FacultyId = null
    );
}