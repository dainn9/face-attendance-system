using api_gateway.Contracts.Face;

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
        private static void AddFile(
            MultipartFormDataContent form,
            IFormFile file,
            string fieldName)
        {
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            form.Add(streamContent, fieldName, file.FileName);
        }
    }
}