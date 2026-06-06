using api_gateway.Contracts;
using api_gateway.Contracts.Users;
using api_gateway.Exceptions;
using BuildingBlocks.Results;

namespace api_gateway.Clients
{
    public class UserClient : BaseHttpClient
    {
        public UserClient(HttpClient httpClient, ILogger<UserClient> logger)
        : base(httpClient, logger) { }

        public async Task CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/internal/users", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            return;
        }

        public async Task<PagedResult<UserPagedDto>> GetUsersAsync(GetUserPagedRequest request, CancellationToken cancellationToken = default)
        {
            var queryString = QueryString.Create(new Dictionary<string, string?>
            {
                ["page"] = request.Page.ToString(),
                ["pageSize"] = request.PageSize.ToString(),
                ["searchQuery"] = request.SearchQuery,
                ["role"] = request.Role?.ToString(),
                ["facultyId"] = request.FacultyId?.ToString()
            });

            var response = await _httpClient.GetAsync($"api/v1/internal/users{queryString}", cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<UserPagedDto>>>(cancellationToken: cancellationToken);
            return result?.Data ?? new PagedResult<UserPagedDto>();
        }

        public async Task<Dictionary<Guid, UserLookupDto>> GetLecturersByIdsAsync(IReadOnlyList<Guid> lecturerIds, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/v1/internal/users/get-lecturers-by-ids",
                lecturerIds,
                cancellationToken
            );

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<Dictionary<Guid, UserLookupDto>>(cancellationToken: cancellationToken);
            return result ?? new Dictionary<Guid, UserLookupDto>();
        }

        public async Task<bool> CheckLecturerExistsAsync(Guid lecturerId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/v1/internal/users/lecturers/{lecturerId}/exists", cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken: cancellationToken);
            return result?.Data ?? throw new InvalidOperationException("User service returned empty response.");
        }

        public async Task<LecturerDto?> GetLecturerByIdAsync(Guid lecturerId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/v1/internal/users/lecturers/{lecturerId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<LecturerDto?>>(cancellationToken: cancellationToken);
            return result?.Data;
        }

        public async Task<Dictionary<Guid, StudentSummaryDto>> GetStudentSummariesByIdsAsync(IReadOnlyList<Guid> studentIds, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/v1/internal/users/get-students-by-ids",
                studentIds,
                cancellationToken
            );

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<Guid, StudentSummaryDto>>>(cancellationToken: cancellationToken);
            return result?.Data ?? new Dictionary<Guid, StudentSummaryDto>();
        }

        public async Task<IReadOnlyList<Guid>> GetExistingStudentIdsAsync(IReadOnlyList<Guid> studentIds, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/v1/internal/users/get-existing-student-ids",
                studentIds,
                cancellationToken
            );

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<Guid>>>(cancellationToken: cancellationToken);
            return result?.Data ?? throw new InvalidOperationException("User service returned empty response.");
        }
    }
}