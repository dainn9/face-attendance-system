namespace api_gateway.Contracts.Face
{
    public record VerifyFaceRequest(
        string Challenge,
        IFormFile? Center,
        IFormFile? ChallengeImage
    );
}