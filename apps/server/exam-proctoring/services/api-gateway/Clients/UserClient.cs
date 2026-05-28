using api_gateway.Contracts;
using api_gateway.Exceptions;

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
    }
}