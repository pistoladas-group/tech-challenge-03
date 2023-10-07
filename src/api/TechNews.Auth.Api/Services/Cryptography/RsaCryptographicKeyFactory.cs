using System.Security.Cryptography;
using TechNews.Auth.Api.Configurations;

namespace TechNews.Auth.Api.Services.Cryptography;

public class RsaCryptographicKeyFactory : ICryptographicKeyFactory
{
    public ICryptographicKey CreateKey()
    {
        var instance = RSA.Create(EnvironmentVariables.KeyCreationSizeInBits);

        return new RsaCryptographicKey(Guid.NewGuid(), DateTime.UtcNow, instance);
    }

    public ICryptographicKey CreateFromPrivateKey(string base64KeyBytes)
    {
        var privateKeyBytes = Convert.FromBase64String(base64KeyBytes);

        var instance = RSA.Create();
        instance.ImportRSAPrivateKey(privateKeyBytes, out _);

        return new RsaCryptographicKey(Guid.NewGuid(), DateTime.UtcNow, instance);
    }
}