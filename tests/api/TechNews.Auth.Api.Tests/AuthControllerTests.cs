using System.Net;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechNews.Auth.Api.Controllers;
using TechNews.Auth.Api.Data;
using TechNews.Auth.Api.Services.KeyRetrievers;
using TechNews.Core.Api.Tests;

namespace TechNews.Auth.Api.Tests;

public class AuthControllerTests : IClassFixture<TestsFixture>
{
    private TestsFixture _testsFixture { get; set; }

    public AuthControllerTests(TestsFixture testsFixture)
    {
        _testsFixture = testsFixture;
    }

    [Fact]
    public async void RegisterUserAsync_ShouldReturnBadRequest_WhenUserAlreadyExists()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

        var requestFake = _testsFixture.GetValidRegisterUserRequestModel();
        var existingUserFake = new User(requestFake.Id ?? Guid.NewGuid(), requestFake.Email, requestFake.UserName);

        if (requestFake.Id is null)
        {
            Assert.Fail($"Arrange not configured correctly. Property {nameof(requestFake.Id)} should not be null.");
        }

        A.CallTo(() => userManagerFake.FindByIdAsync(requestFake.Id.Value.ToString()))
            .Returns(Task.FromResult<User?>(existingUserFake));

        // Act
        var response = await controller.RegisterUserAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal(objectResult?.StatusCode, (int)HttpStatusCode.BadRequest);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "UserAlreadyExists"));
    }
}