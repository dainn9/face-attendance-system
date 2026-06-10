using api_gateway.Exceptions;
using BuildingBlocks.Results;

namespace api_gateway.Clients
{
    public abstract class BaseHttpClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger _logger;

        protected BaseHttpClient(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        protected async Task HandleErrorAsync(HttpResponseMessage response, CancellationToken ct)
        {
            var body = await response.Content.ReadAsStringAsync(ct);

            _logger.LogError(
                "Downstream call failed | Client: {Client} | URL: {URL} | Status: {Status} | Body: {Body}",
                GetType().Name,
                response.RequestMessage?.RequestUri,
                (int)response.StatusCode,
                string.IsNullOrWhiteSpace(body) ? "(empty)" : body
            );

            throw new DownstreamApiException((int)response.StatusCode, body);
        }
    }
}