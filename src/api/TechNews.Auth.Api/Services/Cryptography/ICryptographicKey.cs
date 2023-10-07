using Microsoft.IdentityModel.Tokens;
using TechNews.Auth.Api.Models;

public interface ICryptographicKey
{
    Guid Id { get; }
    DateTime CreationDate { get; }

    bool IsValid();
    SigningCredentials GetSigningCredentials();
    JsonWebKeyModel GetJsonWebKey();
    string GetBase64PrivateKeyBytes();
}