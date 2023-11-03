using System.Net;
using Microsoft.AspNetCore.Mvc;
using FakeItEasy;
using TechNews.Auth.Api.Controllers;
using TechNews.Auth.Api.Models;
using TechNews.Auth.Api.Services.Cryptography;
using TechNews.Auth.Api.Services.KeyRetrievers;

namespace TechNews.Auth.Api.Tests;

public class JwksControllerTests : IClassFixture<TestsFixture>
{
    private TestsFixture _testsFixture { get; set; }

    public JwksControllerTests(TestsFixture testsFixture)
    {
        _testsFixture = testsFixture;
    }

    [Fact(DisplayName = "ShouldReturnOkEmptyJson_WhenNoKeysFound")]
    [Trait("Get JWKS", "")]
    public async void GetJsonWebKeySetsAsync_ShouldReturnOkWithEmptyKeys_WhenNoKeysFound()
    {
        // Arrange
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new JwksController(cryptographicKeyRetrieverFake);

        A.CallTo(() => cryptographicKeyRetrieverFake.GetLastKeysAsync(A<int>._))
        .Returns(Task.FromResult(new List<ICryptographicKey>()));

        // Act
        var response = await controller.GetJsonWebKeySetsAsync();

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = (JwksResponseModel?)objectResult?.Value;

        Assert.Equal((int)HttpStatusCode.OK, objectResult?.StatusCode);
        Assert.Equal(0, apiResponse?.Keys.Count);
    }

    [Fact(DisplayName = "ShouldReturnOkWithKeys_WhenKeysFound")]
    [Trait("Get JWKS", "")]
    public async void GetJsonWebKeySetsAsync_ShouldReturnOkWithKeys_WhenKeysFound()
    {
        // Arrange
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new JwksController(cryptographicKeyRetrieverFake);

        A.CallTo(() => cryptographicKeyRetrieverFake.GetLastKeysAsync(A<int>._))
        .Returns(Task.FromResult(_testsFixture.GetFakeJwksKeys()));

        // Act
        var response = await controller.GetJsonWebKeySetsAsync();

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = (JwksResponseModel?)objectResult?.Value;

        Assert.Equal((int)HttpStatusCode.OK, objectResult?.StatusCode);
        Assert.True(apiResponse?.Keys.Count > 0);
    }
}