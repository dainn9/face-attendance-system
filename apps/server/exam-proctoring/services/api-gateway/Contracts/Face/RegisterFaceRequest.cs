namespace api_gateway.Contracts.Face
{
    public record RegisterFaceRequest(
        IFormFile? Left,
        IFormFile? Center,
        IFormFile? Right
    );
}