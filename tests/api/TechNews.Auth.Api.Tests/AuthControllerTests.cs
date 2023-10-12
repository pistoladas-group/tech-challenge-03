using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
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

        if (requestFake.Id is null)
        {
            Assert.Fail($"Arrange not configured correctly. Property {nameof(requestFake.Id)} should not be null.");
        }

        var existingUserFake = new User(requestFake.Id.Value, requestFake.Email, requestFake.UserName);

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._))
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
        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact]
    public async void RegisterUserAsync_ShouldReturnBadRequest_WhenUserCreationDoesNotSucceed()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

        var requestFake = _testsFixture.GetValidRegisterUserRequestModel();

        if (requestFake.Id is null)
        {
            Assert.Fail($"Arrange not configured correctly. Property {nameof(requestFake.Id)} should not be null.");
        }

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._))
            .Returns(Task.FromResult<User?>(null));

        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._))
           .Returns(Task.FromResult(_testsFixture.GetDefaultIdentityFailure()));

        // Act
        var response = await controller.RegisterUserAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal(objectResult?.StatusCode, (int)HttpStatusCode.BadRequest);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact]
    public async void RegisterUserAsync_ShouldReturnInternalError_WhenUserCreatedIsNotFound()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

        var requestFake = _testsFixture.GetValidRegisterUserRequestModel();

        if (requestFake.Id is null)
        {
            Assert.Fail($"Arrange not configured correctly. Property {nameof(requestFake.Id)} should not be null.");
        }

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._))
            .Returns(Task.FromResult<User?>(null));

        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._))
           .Returns(Task.FromResult(IdentityResult.Success));

        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
           .Returns(Task.FromResult<User?>(null));

        // Act
        var response = await controller.RegisterUserAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal(objectResult?.StatusCode, (int)HttpStatusCode.InternalServerError);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InternalError"));
        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact]
    public async void RegisterUserAsync_ShouldReturnTokenWithAllClaims_WhenUserHasClaimsOrRoles()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var cryptoKeyFake = A.Fake<ICryptographicKey>();
        var httpContextFake = A.Fake<HttpContext>();
        var requestFake = _testsFixture.GetValidRegisterUserRequestModel();

        if (requestFake.Id is null)
        {
            Assert.Fail($"Arrange not configured correctly. Property {nameof(requestFake.Id)} should not be null.");
        }

        var createdUserFake = new User(requestFake.Id.Value, requestFake.Email, requestFake.UserName);

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._))
            .Returns(Task.FromResult<User?>(null));

        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._))
           .Returns(Task.FromResult(IdentityResult.Success));

        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
           .Returns(Task.FromResult<User?>(createdUserFake));

        A.CallTo(() => cryptographicKeyRetrieverFake.GetExistingKeyAsync())
           .Returns(Task.FromResult<ICryptographicKey?>(cryptoKeyFake));

        A.CallTo(() => cryptoKeyFake.GetSigningCredentials())
           .Returns(_testsFixture.GetRsaSigningCredentials());

        A.CallTo(() => httpContextFake.Request.Scheme).Returns("https");
        A.CallTo(() => httpContextFake.Request.Host).Returns(new HostString("localhost:5000"));

        A.CallTo(() => userManagerFake.GetClaimsAsync(createdUserFake))
           .Returns(Task.FromResult<IList<Claim>>(_testsFixture.GetFakeClaims()));

        A.CallTo(() => userManagerFake.GetRolesAsync(createdUserFake))
           .Returns(Task.FromResult<IList<string>>(_testsFixture.GetFakeRoles()));

        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextFake
            }
        };

        // Act
        var response = await controller.RegisterUserAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);
        var accessToken = _testsFixture.GetAccessTokenFromApiResponse(apiResponse);

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(accessToken?.AccessToken) as JwtSecurityToken;

        Assert.Equal(objectResult?.StatusCode, (int)HttpStatusCode.Created);
        Assert.NotNull(apiResponse?.Data);
        Assert.True(accessToken?.ExpiresInSeconds > 0);
        Assert.Equal("at+jwt", accessToken?.TokenType);
        Assert.True(!string.IsNullOrWhiteSpace(accessToken?.AccessToken));

        Assert.True(jsonToken?.Claims.Any(c => c.Type == "claim1" && c.Value == "testClaim1"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "claim2" && c.Value == "testClaim2"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "role" && c.Value == "testRole1"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "role" && c.Value == "testRole2"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "sub"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "email"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "name"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "jti"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "nbf"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "iat"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "iss"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "exp"));

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact]
    public async void RegisterUserAsync_ShouldReturnTokenWithOnlyJwtClaims_WhenUserHasNoClaimsAndRoles()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var cryptoKeyFake = A.Fake<ICryptographicKey>();
        var httpContextFake = A.Fake<HttpContext>();
        var requestFake = _testsFixture.GetValidRegisterUserRequestModel();

        if (requestFake.Id is null)
        {
            Assert.Fail($"Arrange not configured correctly. Property {nameof(requestFake.Id)} should not be null.");
        }

        var createdUserFake = new User(requestFake.Id.Value, requestFake.Email, requestFake.UserName);

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._))
            .Returns(Task.FromResult<User?>(null));

        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._))
           .Returns(Task.FromResult(IdentityResult.Success));

        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
           .Returns(Task.FromResult<User?>(createdUserFake));

        A.CallTo(() => cryptographicKeyRetrieverFake.GetExistingKeyAsync())
           .Returns(Task.FromResult<ICryptographicKey?>(cryptoKeyFake));

        A.CallTo(() => cryptoKeyFake.GetSigningCredentials())
           .Returns(_testsFixture.GetRsaSigningCredentials());

        A.CallTo(() => httpContextFake.Request.Scheme).Returns("https");
        A.CallTo(() => httpContextFake.Request.Host).Returns(new HostString("localhost:5000"));

        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextFake
            }
        };

        // Act
        var response = await controller.RegisterUserAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);
        var accessToken = _testsFixture.GetAccessTokenFromApiResponse(apiResponse);

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(accessToken?.AccessToken) as JwtSecurityToken;

        Assert.Equal(objectResult?.StatusCode, (int)HttpStatusCode.Created);
        Assert.NotNull(apiResponse?.Data);
        Assert.True(accessToken?.ExpiresInSeconds > 0);
        Assert.Equal("at+jwt", accessToken?.TokenType);
        Assert.True(!string.IsNullOrWhiteSpace(accessToken?.AccessToken));

        Assert.True(jsonToken?.Claims.Any(c => c.Type == "sub"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "email"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "name"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "jti"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "nbf"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "iat"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "iss"));
        Assert.True(jsonToken?.Claims.Any(c => c.Type == "exp"));

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact]
    public async void RegisterUserAsync_ShouldReturnInternalError_WhenNoCryptoKeyAvailable()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

        var requestFake = _testsFixture.GetValidRegisterUserRequestModel();

        if (requestFake.Id is null)
        {
            Assert.Fail($"Arrange not configured correctly. Property {nameof(requestFake.Id)} should not be null.");
        }

        var createdUserFake = new User(requestFake.Id.Value, requestFake.Email, requestFake.UserName);

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._))
            .Returns(Task.FromResult<User?>(null));

        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._))
           .Returns(Task.FromResult(IdentityResult.Success));

        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
           .Returns(Task.FromResult<User?>(createdUserFake));

        A.CallTo(() => cryptographicKeyRetrieverFake.GetExistingKeyAsync())
           .Returns(Task.FromResult<ICryptographicKey?>(null));

        // Act
        var response = await controller.RegisterUserAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal(objectResult?.StatusCode, (int)HttpStatusCode.InternalServerError);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InternalError"));
        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact]
    public async void RegisterUserAsync_ShouldReturnCreatedWithToken_WhenRequestIsValid()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var cryptoKeyFake = A.Fake<ICryptographicKey>();
        var httpContextFake = A.Fake<HttpContext>();
        var requestFake = _testsFixture.GetValidRegisterUserRequestModel();

        if (requestFake.Id is null)
        {
            Assert.Fail($"Arrange not configured correctly. Property {nameof(requestFake.Id)} should not be null.");
        }

        var createdUserFake = new User(requestFake.Id.Value, requestFake.Email, requestFake.UserName);

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._))
            .Returns(Task.FromResult<User?>(null));

        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._))
           .Returns(Task.FromResult(IdentityResult.Success));

        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
           .Returns(Task.FromResult<User?>(createdUserFake));

        A.CallTo(() => cryptographicKeyRetrieverFake.GetExistingKeyAsync())
           .Returns(Task.FromResult<ICryptographicKey?>(cryptoKeyFake));

        A.CallTo(() => cryptoKeyFake.GetSigningCredentials())
           .Returns(_testsFixture.GetRsaSigningCredentials());

        A.CallTo(() => httpContextFake.Request.Scheme).Returns("https");
        A.CallTo(() => httpContextFake.Request.Host).Returns(new HostString("localhost:5000"));

        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextFake
            }
        };

        // Act
        var response = await controller.RegisterUserAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);
        var accessToken = _testsFixture.GetAccessTokenFromApiResponse(apiResponse);

        Assert.Equal(objectResult?.StatusCode, (int)HttpStatusCode.Created);
        Assert.NotNull(apiResponse?.Data);
        Assert.True(accessToken?.ExpiresInSeconds > 0);
        Assert.Equal("at+jwt", accessToken?.TokenType);
        Assert.True(!string.IsNullOrWhiteSpace(accessToken?.AccessToken));
        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }
}