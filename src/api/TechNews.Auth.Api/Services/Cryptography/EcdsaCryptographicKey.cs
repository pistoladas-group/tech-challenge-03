using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using TechNews.Auth.Api.Configurations;
using TechNews.Auth.Api.Models;

namespace TechNews.Auth.Api.Services.Cryptography;

public class EcdsaCryptographicKey : ICryptographicKey
{
    public Guid Id { get; private set; }
    public DateTime CreationDate { get; private set; }
    private ECDsa _keyInstance { get; set; }

    public EcdsaCryptographicKey(Guid id, DateTime creationDate, ECDsa instance)
    {
        Id = id;
        CreationDate = creationDate;
        _keyInstance = instance;
    }

    public bool IsValid()
    {
        return (DateTime.UtcNow - CreationDate).TotalDays < EnvironmentVariables.KeyExpirationInDays;
    }

    public SigningCredentials GetSigningCredentials()
    {
        return new SigningCredentials(new ECDsaSecurityKey(_keyInstance), SecurityAlgorithms.EcdsaSha512)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };
    }

    public JsonWebKeyModel GetJsonWebKey()
    {
        var publicParameters = _keyInstance.ExportParameters(false);

        return new JsonWebKeyModel()
        {
            KeyType = "EC",
            KeyId = Id.ToString(),
            Algorithm = "ES512",
            Use = "sig",
            XAxis = Convert.ToBase64String(publicParameters.Q.X ?? Array.Empty<byte>()),
            YAxis = Convert.ToBase64String(publicParameters.Q.Y ?? Array.Empty<byte>()),
            Curve = "P-512"
        };
    }

    public string GetBase64PrivateKeyBytes()
    {
        return Convert.ToBase64String(_keyInstance.ExportECPrivateKey(), Base64FormattingOptions.None);
    }
}