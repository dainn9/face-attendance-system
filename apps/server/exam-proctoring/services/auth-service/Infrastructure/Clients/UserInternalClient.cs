// using auth_service.Application.Abstractions.Clients;
// using auth_service.Application.Contracts;

// namespace auth_service.Infrastructure.Clients
// {
//     public class UserInternalClient : IUserInternalClient
//     {
//         private readonly HttpClient _httpClient;

//         public UserInternalClient(HttpClient httpClient)
//         {
//             _httpClient = httpClient;
//         }

//         public async Task CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
//         {
//             var response = await _httpClient.PostAsJsonAsync("api/v1/internal/users/create", request, cancellationToken);

//             if (response.IsSuccessStatusCode)
//                 return;

//             var body = await response.Content.ReadAsStringAsync(cancellationToken);

//             throw new HttpRequestException(
//                 $"User-service create failed. Status: {(int)response.StatusCode}. Body: {body}");
//         }
//     }
// }