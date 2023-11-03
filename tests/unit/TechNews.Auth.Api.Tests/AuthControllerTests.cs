using System.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FakeItEasy;
using TechNews.Auth.Api.Controllers;
using TechNews.Auth.Api.Data;
using TechNews.Auth.Api.Models;
using TechNews.Auth.Api.Services.Cryptography;
using TechNews.Auth.Api.Services.KeyRetrievers;

namespace TechNews.Auth.Api.Tests;

public class AuthControllerTests : IClassFixture<TestsFixture>
{
    private TestsFixture _testsFixture { get; set; }

    public AuthControllerTests(TestsFixture testsFixture)
    {
        _testsFixture = testsFixture;
    }

    [Fact(DisplayName = "ShouldReturnBadRequest_WhenUserAlreadyExists")]
    [Trait("Register User", "")]
    public async void RegisterUserAsync_ShouldReturnBadRequest_WhenUserAlreadyExists()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);
        var requestFake = _testsFixture.GetValidRegisterUserRequestModel();

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._))
        .Returns(Task.FromResult<User?>(_testsFixture.GetFakeUser()));

        // Act
        var response = await controller.RegisterUserAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal((int)HttpStatusCode.BadRequest, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "UserAlreadyExists"));
        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact(DisplayName = "ShouldReturnBadRequest_WhenUserCreationDoesNotSucceed")]
    [Trait("Register User", "")]
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

        Assert.Equal((int)HttpStatusCode.BadRequest, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact(DisplayName = "ShouldReturnInternalError_WhenUserCreatedIsNotFound")]
    [Trait("Register User", "")]
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

        Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InternalError"));
        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact(DisplayName = "ShouldReturnTokenWithAllClaims_WhenUserHasClaimsOrRoles")]
    [Trait("Register User", "")]
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
           .Returns(Task.FromResult(_testsFixture.GetFakeClaims()));

        A.CallTo(() => userManagerFake.GetRolesAsync(createdUserFake))
           .Returns(Task.FromResult(_testsFixture.GetFakeRoles()));

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

        Assert.Equal((int)HttpStatusCode.Created, objectResult?.StatusCode);
        Assert.NotNull(apiResponse?.Data);
        Assert.Null(apiResponse?.Errors);
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

    [Fact(DisplayName = "ShouldReturnTokenWithOnlyJwtClaims_WhenUserHasNoClaimsAndRoles")]
    [Trait("Register User", "")]
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

        Assert.Equal((int)HttpStatusCode.Created, objectResult?.StatusCode);
        Assert.NotNull(apiResponse?.Data);
        Assert.Null(apiResponse?.Errors);
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

    [Fact(DisplayName = "ShouldReturnInternalError_WhenNoCryptoKeyAvailable")]
    [Trait("Register User", "")]
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

        Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InternalError"));
        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact(DisplayName = "ShouldReturnCreatedWithToken_WhenRequestIsValid")]
    [Trait("Register User", "")]
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

        Assert.Equal((int)HttpStatusCode.Created, objectResult?.StatusCode);
        Assert.NotNull(apiResponse?.Data);
        Assert.Null(apiResponse?.Errors);
        Assert.True(accessToken?.ExpiresInSeconds > 0);
        Assert.Equal("at+jwt", accessToken?.TokenType);
        Assert.True(!string.IsNullOrWhiteSpace(accessToken?.AccessToken));
        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.CreateAsync(A<User>._, A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact(DisplayName = "ShouldReturnBadRequest_WhenUserNotFound")]
    [Trait("Login User", "")]
    public async void LoginAsync_ShouldReturnBadRequest_WhenUserNotFound()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

        var requestFake = _testsFixture.GetValidLoginRequestModel();

        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
            .Returns(Task.FromResult<User?>(null));

        // Act
        var response = await controller.LoginAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal((int)HttpStatusCode.BadRequest, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InvalidRequest"));
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact(DisplayName = "ShouldReturnBadRequest_WhenUserHasNoUserName")]
    [Trait("Login User", "")]
    public async void LoginAsync_ShouldReturnBadRequest_WhenUserHasNoUserName()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

        var requestFake = _testsFixture.GetValidLoginRequestModel();

        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
            .Returns(Task.FromResult<User?>(new User(Guid.NewGuid(), requestFake.Email, null)));

        // Act
        var response = await controller.LoginAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal((int)HttpStatusCode.BadRequest, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InvalidRequest"));
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact(DisplayName = "ShouldReturnForbidden_WhenUserLockedOut")]
    [Trait("Login User", "")]
    public async void LoginAsync_ShouldReturnForbidden_WhenUserLockedOut()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

        var requestFake = _testsFixture.GetValidLoginRequestModel();

        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
            .Returns(Task.FromResult<User?>(_testsFixture.GetFakeUser()));

        A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true))
            .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.LockedOut));

        // Act
        var response = await controller.LoginAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal((int)HttpStatusCode.Forbidden, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "LockedUser"));
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact(DisplayName = "ShouldReturnBadRequest_WhenSignInFails")]
    [Trait("Login User", "")]
    public async void LoginAsync_ShouldReturnBadRequest_WhenSignInFails()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

        var requestFake = _testsFixture.GetValidLoginRequestModel();

        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
            .Returns(Task.FromResult<User?>(_testsFixture.GetFakeUser()));

        A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true))
            .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Failed));

        // Act
        var response = await controller.LoginAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal((int)HttpStatusCode.BadRequest, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InvalidRequest"));
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact(DisplayName = "ShouldReturnInternalError_WhenNoCryptoKeyAvailable")]
    [Trait("Login User", "")]
    public async void LoginAsync_ShouldReturnInternalError_WhenNoCryptoKeyAvailable()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

        var requestFake = _testsFixture.GetValidLoginRequestModel();

        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
            .Returns(Task.FromResult<User?>(_testsFixture.GetFakeUser()));

        A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true))
            .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));

        A.CallTo(() => cryptographicKeyRetrieverFake.GetExistingKeyAsync())
           .Returns(Task.FromResult<ICryptographicKey?>(null));

        // Act
        var response = await controller.LoginAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InternalError"));
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact(DisplayName = "ShouldReturnOkWithToken_WhenRequestIsValid")]
    [Trait("Login User", "")]
    public async void LoginAsync_ShouldReturnOkWithToken_WhenRequestIsValid()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var cryptoKeyFake = A.Fake<ICryptographicKey>();
        var httpContextFake = A.Fake<HttpContext>();
        var requestFake = _testsFixture.GetValidLoginRequestModel();

        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
            .Returns(Task.FromResult<User?>(_testsFixture.GetFakeUser()));

        A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true))
            .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));

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
        var response = await controller.LoginAsync(requestFake);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);
        var accessToken = _testsFixture.GetAccessTokenFromApiResponse(apiResponse);

        Assert.Equal((int)HttpStatusCode.OK, objectResult?.StatusCode);
        Assert.NotNull(apiResponse?.Data);
        Assert.Null(apiResponse?.Errors);
        Assert.True(accessToken?.ExpiresInSeconds > 0);
        Assert.Equal("at+jwt", accessToken?.TokenType);
        Assert.True(!string.IsNullOrWhiteSpace(accessToken?.AccessToken));
        A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
        A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    }

    [Fact(DisplayName = "ShouldReturnBadRequest_WhenGuidEmpty")]
    [Trait("Get User", "")]
    public async void GetUser_ShouldReturnBadRequest_WhenGuidEmpty()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

        // Act
        var response = await controller.GetUser(Guid.Empty);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal((int)HttpStatusCode.BadRequest, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InvalidUser"));
    }

    [Fact(DisplayName = "ShouldReturnNotFound_WhenUserNotFound")]
    [Trait("Get User", "")]
    public async void GetUser_ShouldReturnNotFound_WhenUserNotFound()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._))
        .Returns(Task.FromResult<User?>(null));

        // Act
        var response = await controller.GetUser(Guid.NewGuid());

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal((int)HttpStatusCode.NotFound, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "UserNotFound"));
    }

    [Fact(DisplayName = "ShouldReturnOk_WhenUserFound")]
    [Trait("Get User", "")]
    public async void GetUser_ShouldReturnOk_WhenUserFound()
    {
        // Arrange
        var userManagerFake = A.Fake<UserManager<User>>();
        var signInManagerFake = A.Fake<SignInManager<User>>();
        var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
        var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);
        var fakeUser = _testsFixture.GetFakeUser();

        A.CallTo(() => userManagerFake.FindByIdAsync(A<string>._))
        .Returns(Task.FromResult<User?>(fakeUser));

        // Act
        var response = await controller.GetUser(fakeUser.Id);

        // Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal((int)HttpStatusCode.OK, objectResult?.StatusCode);
        Assert.NotNull(apiResponse?.Data);
        Assert.Null(apiResponse?.Errors);
        Assert.Equal(fakeUser.Id, ((GetUserResponseModel?)apiResponse?.Data)?.Id);
    }
}