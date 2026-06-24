using Microsoft.IdentityModel.Tokens;

namespace auth_service.Application.Abstractions.Security
{
    public interface IJwtKeyProvider
    {
        SigningCredentials GetSigningCredentials();
    }
}