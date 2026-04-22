using System.Security.Cryptography;
using auth_service.Application.Abstractions.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace auth_service.Infrastructure.Security.Jwt
{
    public class JwtKeyProvider : IJwtKeyProvider
    {
        private readonly RSA _rsa;
        private readonly JwtOptions _options;

        public JwtKeyProvider(IOptions<JwtOptions> options)
        {
            _options = options.Value;

            var privatePath = _options.PrivateKeyPath;
            var privatePem = File.ReadAllText(privatePath);

            _rsa = RSA.Create();
            _rsa.ImportFromPem(privatePem);
        }

        public RsaSecurityKey GetPublicKey()
        {
            return new RsaSecurityKey(_rsa.ExportParameters(false));
        }

        public SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(new RsaSecurityKey(_rsa), SecurityAlgorithms.RsaSha256);
        }
    }
}