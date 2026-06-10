using api_gateway.Contracts;
using api_gateway.Exceptions;

namespace api_gateway.Clients
{
    public class AuthClient
    {
        private readonly HttpClient _httpClient;

        public AuthClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Guid> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/internal/auth/accounts", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content
                    .ReadAsStringAsync(cancellationToken);

                throw new DownstreamApiException(
                    (int)response.StatusCode,
                    body);
            }
            var result = await response.Content
                   .ReadFromJsonAsync<CreateAccountResponse>(
                       cancellationToken: cancellationToken);

            if (result is null)
                throw new InvalidOperationException(
                    "Auth service returned empty response.");

            return result.UserId;
        }

        public async Task DeleteAccountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/v1/internal/auth/accounts/{userId}", cancellationToken);

            if (response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new DownstreamApiException((int)response.StatusCode, body);
        }

        public async Task<Dictionary<Guid, bool>> GetStatusByIdsAsync(IReadOnlyList<Guid> userIds, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/v1/internal/auth/accounts/status",
                userIds,
                cancellationToken
            );

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Dictionary<Guid, bool>>(cancellationToken: cancellationToken);
                return result ?? new Dictionary<Guid, bool>();
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new DownstreamApiException((int)response.StatusCode, body);
        }
    }
}