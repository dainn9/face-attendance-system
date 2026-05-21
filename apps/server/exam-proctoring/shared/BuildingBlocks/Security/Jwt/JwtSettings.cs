namespace BuildingBlocks.Security.Jwt
{
    public class JwtSettings
    {
        public string PublicKeyPath { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
    }
}