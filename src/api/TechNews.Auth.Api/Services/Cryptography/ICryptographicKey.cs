using Microsoft.IdentityModel.Tokens;
using TechNews.Auth.Api.Models;

namespace TechNews.Auth.Api.Services.Cryptography;

public interface ICryptographicKey
{
    Guid Id { get; }
    DateTime CreationDate { get; }

    bool IsValid();
    SigningCredentials GetSigningCredentials();
    JsonWebKeyModel GetJsonWebKey();
    string GetBase64PrivateKeyBytes();
}