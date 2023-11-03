using System.Security.Cryptography;
using FakeItEasy;
using TechNews.Auth.Api.Services.Cryptography;

namespace TechNews.Auth.Api.Tests;

public class EcdsaCryptographicKeyTests : IClassFixture<TestsFixture>
{
    private TestsFixture _testsFixture { get; set; }

    public EcdsaCryptographicKeyTests(TestsFixture testsFixture)
    {
        _testsFixture = testsFixture;
    }

    [Fact(DisplayName = "ShouldReturnNewKey_WhenCalled")]
    [Trait("ECDSA Key Factory", "")]
    public void CreateKey_ShouldReturnNewKey_WhenCalled()
    {
        // Arrange
        var factory = new EcdsaCryptographicKeyFactory();

        // Act
        var key = factory.CreateKey();

        // Assert
        var jwk = key.GetJsonWebKey();

        Assert.True(key.IsValid());
        Assert.Equal("EC", jwk.KeyType);
        Assert.Equal("ES512", jwk.Algorithm);
        Assert.False(string.IsNullOrWhiteSpace(jwk.XAxis));
        Assert.False(string.IsNullOrWhiteSpace(jwk.YAxis));
        Assert.False(string.IsNullOrWhiteSpace(jwk.Curve));
    }

    [Fact(DisplayName = "ShouldReturnSameKey_WhenValidPrivateKeyProvided")]
    [Trait("ECDSA Key Factory", "")]
    public void CreateFromPrivateKey_ShouldReturnSameKey_WhenValidPrivateKeyProvided()
    {
        // Arrange
        var factory = new EcdsaCryptographicKeyFactory();
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
        Assert.Equal("EC", jwk.KeyType);
        Assert.Equal("ES512", jwk.Algorithm);
        Assert.False(string.IsNullOrWhiteSpace(jwk.XAxis));
        Assert.False(string.IsNullOrWhiteSpace(jwk.YAxis));
        Assert.False(string.IsNullOrWhiteSpace(jwk.Curve));
    }

    [Fact(DisplayName = "ShouldReturnTrue_WhenKeyIsValid")]
    [Trait("ECDSA Key", "")]
    public void IsValid_ShouldReturnTrue_WhenKeyIsValid()
    {
        // Arrange
        var ecdsaFake = A.Fake<ECDsa>();
        var key = new EcdsaCryptographicKey(Guid.NewGuid(), DateTime.UtcNow, ecdsaFake);

        // Act
        var result = key.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact(DisplayName = "ShouldReturnFalse_WhenKeyIsNotValid")]
    [Trait("ECDSA Key", "")]
    public void IsValid_ShouldReturnFalse_WhenKeyIsNotValid()
    {
        // Arrange
        var ecdsaFake = A.Fake<ECDsa>();
        var key = new EcdsaCryptographicKey(Guid.NewGuid(), DateTime.UtcNow.AddDays(-30), ecdsaFake);

        // Act
        var result = key.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "ShouldReturnSigningCredentials_WhenKeyIsValid")]
    [Trait("ECDSA Key", "")]
    public void GetSigningCredentials_ShouldReturnSigningCredentials_WhenKeyIsValid()
    {
        // Arrange
        var ecdsaFake = A.Fake<ECDsa>();
        var key = new EcdsaCryptographicKey(Guid.NewGuid(), DateTime.UtcNow, ecdsaFake);

        // Act
        var signingCredentials = key.GetSigningCredentials();

        // Assert
        Assert.NotNull(signingCredentials);
        Assert.Equal("ES512", signingCredentials.Algorithm);
        Assert.NotNull(signingCredentials.Key);
    }

    [Fact(DisplayName = "ShouldReturnJwk_WhenKeyIsValid")]
    [Trait("ECDSA Key", "")]
    public void GetJsonWebKey_ShouldReturnJwk_WhenKeyIsValid()
    {
        // Arrange
        var factory = new EcdsaCryptographicKeyFactory();
        var key = factory.CreateKey();

        // Act
        var jwk = key.GetJsonWebKey();

        // Assert
        Assert.Equal("EC", jwk.KeyType);
        Assert.Equal("ES512", jwk.Algorithm);
        Assert.False(string.IsNullOrWhiteSpace(jwk.XAxis));
        Assert.False(string.IsNullOrWhiteSpace(jwk.YAxis));
        Assert.False(string.IsNullOrWhiteSpace(jwk.Curve));
    }

    [Fact(DisplayName = "ShouldReturnPrivateKey_WhenKeyIsValid")]
    [Trait("ECDSA Key", "")]
    public void GetBase64PrivateKeyBytes_ShouldReturnPrivateKey_WhenKeyIsValid()
    {
        // Arrange
        var factory = new EcdsaCryptographicKeyFactory();
        var key = factory.CreateKey();

        // Act
        var privateKey = key.GetBase64PrivateKeyBytes();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(privateKey));
    }
}