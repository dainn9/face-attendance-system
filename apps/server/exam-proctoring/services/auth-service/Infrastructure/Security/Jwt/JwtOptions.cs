namespace auth_service.Infrastructure.Security.Jwt
{
    public class JwtOptions
    {
        public string PrivateKeyPath { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int AccessTokenMinutes { get; set; }
        public int RefreshTokenDays { get; set; }
    }
}