using api_gateway.Contracts.Attendance;
using api_gateway.Contracts.Face;
using BuildingBlocks.Results;

namespace api_gateway.Clients
{
    public class FaceClient : BaseHttpClient
    {
        public FaceClient(HttpClient httpClient, ILogger<FaceClient> logger)
            : base(httpClient, logger) { }

        public async Task RegisterFaceAsync(
            Guid userId,
            RegisterFaceRequest request,
            CancellationToken cancellationToken = default)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(userId.ToString()), "user_id");

            AddFile(form, request.Left!, "left");
            AddFile(form, request.Center!, "center");
            AddFile(form, request.Right!, "right");

            var response = await _httpClient.PostAsync("api/v1/internal/faces/register", form, cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            return;
        }

        public async Task<bool> GetStatusAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/v1/internal/faces/status/{userId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(cancellationToken: cancellationToken);
            return result?.Data ?? false;
        }

        public async Task<ChallengeDto> GetChallengeAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/v1/internal/faces/challenge", cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ChallengeDto>>(cancellationToken: cancellationToken);
            return result?.Data ?? new ChallengeDto(string.Empty);
        }

        public async Task<VerifyFaceResponse> VerifyFaceAsync(
            Guid userId,
            VerifyFaceRequest request,
            CancellationToken cancellationToken = default)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(userId.ToString()), "user_id");
            form.Add(new StringContent(request.Challenge), "challenge");

            AddFile(form, request.Video!, "video");

            var response = await _httpClient.PostAsync("api/v1/internal/faces/verify-liveness", form, cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleErrorAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<VerifyFaceResponse>>(cancellationToken: cancellationToken);
            return result?.Data ?? new VerifyFaceResponse(false, 0);
        }

        private static void AddFile(
            MultipartFormDataContent form,
            IFormFile file,
            string fieldName)
        {
            var streamContent = new StreamContent(file.OpenReadStream());
            var mediaType = file.ContentType.Split(';')[0];
            if (!mediaType.StartsWith("image/") && !mediaType.StartsWith("video/"))
                throw new ArgumentException("Invalid media type");

            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);
            form.Add(streamContent, fieldName, file.FileName);
        }
    }
}