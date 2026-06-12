namespace api_gateway.Contracts.Face
{
    public record VerifyFaceResponse(
        bool Match,
        double Score
    );
}