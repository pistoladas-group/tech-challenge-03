using System.Security.Cryptography;

namespace TechNews.Auth.Api.Services.Cryptography;

public class EcdsaCryptographicKeyFactory : ICryptographicKeyFactory
{
    public ICryptographicKey CreateKey()
    {
        var instance = ECDsa.Create(ECCurve.NamedCurves.nistP521);

        return new EcdsaCryptographicKey(Guid.NewGuid(), DateTime.UtcNow, instance);
    }

    public ICryptographicKey CreateFromPrivateKey(string base64KeyBytes)
    {
        var privateKeyBytes = Convert.FromBase64String(base64KeyBytes);

        var instance = ECDsa.Create();
        instance.ImportECPrivateKey(privateKeyBytes, out _);

        return new EcdsaCryptographicKey(Guid.NewGuid(), DateTime.UtcNow, instance);
    }
}