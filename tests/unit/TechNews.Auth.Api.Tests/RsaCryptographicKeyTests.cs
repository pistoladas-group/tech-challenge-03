using System.Security.Cryptography;
using FakeItEasy;
using TechNews.Auth.Api.Services.Cryptography;

namespace TechNews.Auth.Api.Tests;

public class RsaCryptographicKeyTests : IClassFixture<TestsFixture>
{
    private TestsFixture _testsFixture { get; set; }

    public RsaCryptographicKeyTests(TestsFixture testsFixture)
    {
        _testsFixture = testsFixture;
    }

    [Fact(DisplayName = "ShouldReturnNewKey_WhenCalled")]
    [Trait("RSA Key Factory", "")]
    public void CreateKey_ShouldReturnNewKey_WhenCalled()
    {
        // Arrange
        var factory = new RsaCryptographicKeyFactory();

        // Act
        var key = factory.CreateKey();

        // Assert
        var jwk = key.GetJsonWebKey();

        Assert.True(key.IsValid());
        Assert.Equal("RSA", jwk.KeyType);
        Assert.Equal("RS256", jwk.Algorithm);
        Assert.False(string.IsNullOrWhiteSpace(jwk.Modulus));
        Assert.False(string.IsNullOrWhiteSpace(jwk.Exponent));
    }

    [Fact(DisplayName = "ShouldReturnSameKey_WhenValidPrivateKeyProvided")]
    [Trait("RSA Key Factory", "")]
    public void CreateFromPrivateKey_ShouldReturnSameKey_WhenValidPrivateKeyProvided()
    {
        // Arrange
        var factory = new RsaCryptographicKeyFactory();
        var newKey = factory.CreateKey();
        var privateKey = newKey.GetBase64PrivateKeyBytes();

        // Act
        var importedKey = factory.CreateFromPrivateKey(privateKey);

        // Assert
        var jwk = importedKey.GetJsonWebKey();
        var importedPrivateKey = importedKey.GetBase64PrivateKeyBytes();

        // Check if imported key is the same as the one that originated it
        Assert.Equal(privateKey, importedPrivateKey);

        Assert.True(importedKey.IsValid());
        Assert.Equal("RSA", jwk.KeyType);
        Assert.Equal("RS256", jwk.Algorithm);
        Assert.False(string.IsNullOrWhiteSpace(jwk.Modulus));
        Assert.False(string.IsNullOrWhiteSpace(jwk.Exponent));
    }

    [Fact(DisplayName = "ShouldReturnTrue_WhenKeyIsValid")]
    [Trait("RSA Key", "")]
    public void IsValid_ShouldReturnTrue_WhenKeyIsValid()
    {
        // Arrange
        var rsaFake = A.Fake<RSA>();
        var key = new RsaCryptographicKey(Guid.NewGuid(), DateTime.UtcNow, rsaFake);

        // Act
        var result = key.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact(DisplayName = "ShouldReturnFalse_WhenKeyIsNotValid")]
    [Trait("RSA Key", "")]
    public void IsValid_ShouldReturnFalse_WhenKeyIsNotValid()
    {
        // Arrange
        var rsaFake = A.Fake<RSA>();
        var key = new RsaCryptographicKey(Guid.NewGuid(), DateTime.UtcNow.AddDays(-30), rsaFake);

        // Act
        var result = key.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "ShouldReturnSigningCredentials_WhenKeyIsValid")]
    [Trait("RSA Key", "")]
    public void GetSigningCredentials_ShouldReturnSigningCredentials_WhenKeyIsValid()
    {
        // Arrange
        var rsaFake = A.Fake<RSA>();
        var key = new RsaCryptographicKey(Guid.NewGuid(), DateTime.UtcNow, rsaFake);

        // Act
        var signingCredentials = key.GetSigningCredentials();

        // Assert
        Assert.NotNull(signingCredentials);
        Assert.Equal("RS256", signingCredentials.Algorithm);
        Assert.NotNull(signingCredentials.Key);
    }

    [Fact(DisplayName = "ShouldReturnJwk_WhenKeyIsValid")]
    [Trait("RSA Key", "")]
    public void GetJsonWebKey_ShouldReturnJwk_WhenKeyIsValid()
    {
        // Arrange
        var factory = new RsaCryptographicKeyFactory();
        var key = factory.CreateKey();

        // Act
        var jwk = key.GetJsonWebKey();

        // Assert
        Assert.Equal("RSA", jwk.KeyType);
        Assert.Equal("RS256", jwk.Algorithm);
        Assert.False(string.IsNullOrWhiteSpace(jwk.Modulus));
        Assert.False(string.IsNullOrWhiteSpace(jwk.Exponent));
    }

    [Fact(DisplayName = "ShouldReturnPrivateKey_WhenKeyIsValid")]
    [Trait("RSA Key", "")]
    public void GetBase64PrivateKeyBytes_ShouldReturnPrivateKey_WhenKeyIsValid()
    {
        // Arrange
        var factory = new RsaCryptographicKeyFactory();
        var key = factory.CreateKey();

        // Act
        var privateKey = key.GetBase64PrivateKeyBytes();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(privateKey));
    }
}