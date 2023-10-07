using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using TechNews.Auth.Api.Configurations;
using TechNews.Auth.Api.Models;

public class RsaCryptographicKey : ICryptographicKey
{
    public Guid Id { get; private set; }
    public DateTime CreationDate { get; private set; }
    private RSA _keyInstance { get; set; }

    public RsaCryptographicKey(Guid id, DateTime creationDate, RSA instance)
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
        return new SigningCredentials(new RsaSecurityKey(_keyInstance), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };
    }

    public JsonWebKeyModel GetJsonWebKey()
    {
        var publicParameters = _keyInstance.ExportParameters(false);

        return new JsonWebKeyModel()
        {
            KeyType = "RSA",
            KeyId = Id.ToString(),
            Algorithm = "RS256",
            Use = "sig",
            Modulus = Base64UrlEncoder.Encode(publicParameters.Modulus),
            Exponent = Base64UrlEncoder.Encode(publicParameters.Exponent)
        };
    }

    public string GetBase64PrivateKeyBytes()
    {
        return Convert.ToBase64String(_keyInstance.ExportRSAPrivateKey(), Base64FormattingOptions.None);
    }
}