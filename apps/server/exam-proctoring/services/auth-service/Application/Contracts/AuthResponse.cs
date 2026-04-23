namespace auth_service.Application.Contracts
{
    public record AuthResponse(
        string AccessToken,
        string RefreshToken,
        TimeSpan AccessTokenExpiresIn,
        TimeSpan RefreshTokenExpiresIn
    // long AccessTokenExpiresAt,
    // long RefreshTokenExpiresAt
    );
}