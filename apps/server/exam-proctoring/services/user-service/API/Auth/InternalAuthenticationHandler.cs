using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace user_service.API.Auth
{
    public class InternalAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "Internal";
        private const string HeaderName = "X-Internal-Api-Key";
        private readonly string? _expectedApiKey;

        public InternalAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration
        ) : base(options, logger, encoder)
        {
            _expectedApiKey = configuration["InternalAuth:ApiKey"];
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (string.IsNullOrWhiteSpace(_expectedApiKey))
            {
                Logger.LogError("Internal authentication is not configured.");
                return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
            }

            if (!Request.Headers.TryGetValue(HeaderName, out var apiKey))
            {
                Logger.LogWarning(
                    "Internal auth failed: missing header from {IP}",
                    Context.Connection.RemoteIpAddress);

                return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
            }

            if (!IsValidApiKey(apiKey.ToString(), _expectedApiKey))
            {
                Logger.LogWarning(
                    "Internal auth failed: invalid API key from {IP}",
                    Context.Connection.RemoteIpAddress);

                return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
            }

            var claims = new[]
            {
              new Claim(ClaimTypes.Name, "internal-service"),
              new Claim("scope", "internal")
          };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private static bool IsValidApiKey(string providedApiKey, string expectedApiKey)
        {
            var providedBytes = Encoding.UTF8.GetBytes(providedApiKey);
            var expectedBytes = Encoding.UTF8.GetBytes(expectedApiKey);

            return CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes);
        }
    }
}