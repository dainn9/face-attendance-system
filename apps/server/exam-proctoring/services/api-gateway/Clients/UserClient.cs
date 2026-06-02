using System.Text.Json;
using api_gateway.Contracts;
using api_gateway.Contracts.Users;
using api_gateway.Contracts.Userss;
using api_gateway.Exceptions;
using BuildingBlocks.Results;

namespace api_gateway.Clients
{
    public class UserClient
    {
        private readonly HttpClient _httpClient;

        public UserClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/internal/users", request, cancellationToken);

            if (response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new DownstreamApiException((int)response.StatusCode, body);
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

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<UserPagedDto>>>(cancellationToken: cancellationToken);
                return result?.Data ?? new PagedResult<UserPagedDto>();
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new DownstreamApiException((int)response.StatusCode, body);
        }
    }
}