namespace TechNews.Auth.Api.Services.Cryptography;

public interface ICryptographicKeyFactory
{
    ICryptographicKey CreateKey();
    ICryptographicKey CreateFromPrivateKey(string base64KeyBytes);
}