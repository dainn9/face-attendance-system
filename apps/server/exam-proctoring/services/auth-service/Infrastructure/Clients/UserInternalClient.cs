using auth_service.Application.Abstractions.Clients;
using auth_service.Application.Contracts;
using SharedKernel.Core.Enums;

namespace auth_service.Infrastructure.Clients
{
    public class UserInternalClient : IUserInternalClient
    {
        private readonly HttpClient _httpClient;

        public UserInternalClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateUserAsync(Guid userId, string fullName, Gender gender, DateOnly dateOfBirth, string email, CancellationToken cancellationToken = default)
        {
            var request = new CreateUserProfileRequest(
                UserId: userId,
                FullName: fullName,
                Gender: gender,
                DateOfBirth: dateOfBirth,
                Email: email
            );

            var response = await _httpClient.PostAsJsonAsync("/internal/users", request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
