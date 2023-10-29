using System.Net;
using System.IdentityModel.Tokens.Jwt;
using Bogus;

namespace TechNews.Auth.Api.IntegrationTests;

[Collection(nameof(TestsFixtureCollection))]
public class AuthControllerTests
{
    private readonly TestsFixture<Program> _testsFixture;

    public AuthControllerTests(TestsFixture<Program> testsFixture)
    {
        _testsFixture = testsFixture;
    }

    [Fact(DisplayName = "ShouldReturnBadRequest_WhenUserAlreadyExists")]
    [Trait("Register User", "")]
    public async void RegisterUserAsync_ShouldReturnBadRequest_WhenUserAlreadyExists()
    {
        // Arrange
        var user = await _testsFixture.RegisterUserAsync();

        // Act
        var httpResponse = await _testsFixture.Client.PostAsync("api/auth/user", _testsFixture.ConvertToContentInJson(user));

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.GetResponseFromString(responseText);

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse?.StatusCode);
        Assert.Null(response?.Data);
        Assert.True(response?.Errors?.Count > 0);
        Assert.True(response?.Errors?.Any(x => x.ErrorCode == "UserAlreadyExists"));
    }

    [Fact(DisplayName = "ShouldReturnBadRequest_WhenUserCreationDoesNotSucceed")]
    [Trait("Register User", "")]
    public async void RegisterUserAsync_ShouldReturnBadRequest_WhenUserCreationDoesNotSucceed()
    {
        // Arrange
        var user = _testsFixture.GetValidRegisterUserRequestModel();
        user.Password = "weakPass";

        // Act
        var httpResponse = await _testsFixture.Client.PostAsync("api/auth/user", _testsFixture.ConvertToContentInJson(user));

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.GetResponseFromString(responseText);

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse?.StatusCode);
        Assert.Null(response?.Data);
        Assert.True(response?.Errors?.Count > 0);
        Assert.True(response?.Errors?.Any(x => x.Description == "The Password field must have at least one digit, one lowercase, one uppercase and a special character"));
        Assert.True(response?.Errors?.Any(x => x.Description == "The passwords does not match"));
    }

    [Fact(DisplayName = "ShouldReturnTokenWithOnlyJwtClaims_WhenUserHasNoClaimsAndRoles")]
    [Trait("Register User", "")]
    public async void RegisterUserAsync_ShouldReturnTokenWithOnlyJwtClaims_WhenUserHasNoClaimsAndRoles()
    {
        // Arrange
        var user = _testsFixture.GetValidRegisterUserRequestModel();

        // Act
        var httpResponse = await _testsFixture.Client.PostAsync("api/auth/user", _testsFixture.ConvertToContentInJson(user));

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.GetResponseFromString(responseText);
        var accessToken = _testsFixture.GetAccessTokenFromAppResponse(response);

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(accessToken?.AccessToken) as JwtSecurityToken;

        Assert.Equal(HttpStatusCode.Created, httpResponse?.StatusCode);
        Assert.NotNull(response?.Data);
        Assert.Null(response?.Errors);
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
    }

    [Fact(DisplayName = "ShouldReturnCreatedWithToken_WhenRequestIsValid")]
    [Trait("Register User", "")]
    public async void RegisterUserAsync_ShouldReturnCreatedWithToken_WhenRequestIsValid()
    {
        // Arrange
        var user = _testsFixture.GetValidRegisterUserRequestModel();

        //Act
        var httpResponse = await _testsFixture.Client.PostAsync("api/auth/user", _testsFixture.ConvertToContentInJson(user));

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.GetResponseFromString(responseText);
        var accessToken = _testsFixture.GetAccessTokenFromAppResponse(response);

        Assert.Equal(HttpStatusCode.Created, httpResponse?.StatusCode);
        Assert.NotNull(response?.Data);
        Assert.Null(response?.Errors);
        Assert.True(accessToken?.ExpiresInSeconds > 0);
        Assert.Equal("at+jwt", accessToken?.TokenType);
        Assert.True(!string.IsNullOrWhiteSpace(accessToken?.AccessToken));
    }

    [Fact(DisplayName = "ShouldReturnBadRequest_WhenUserNotFound")]
    [Trait("Login User", "")]
    public async void LoginAsync_ShouldReturnBadRequest_WhenUserNotFound()
    {
        // Arrange
        var user = _testsFixture.GetValidLoginRequestModel();

        // Act
        var httpResponse = await _testsFixture.Client.PostAsync("api/auth/user/login", _testsFixture.ConvertToContentInJson(user));

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.GetResponseFromString(responseText);

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse?.StatusCode);
        Assert.Null(response?.Data);
        Assert.True(response?.Errors?.Count > 0);
        Assert.True(response?.Errors?.Any(x => x.ErrorCode == "InvalidRequest"));
    }

    [Fact(DisplayName = "ShouldReturnForbidden_WhenUserLockedOut")]
    [Trait("Login User", "")]
    public async void LoginAsync_ShouldReturnForbidden_WhenUserLockedOut()
    {
        // Arrange
        var user = await _testsFixture.RegisterUserAsync();
        user.Password = new Faker().Internet.Password(length: 8, memorable: false, prefix: @"1aA@-");
        await _testsFixture.AttemptLoginAsync(user, 3);

        // Act
        var httpResponse = await _testsFixture.Client.PostAsync("api/auth/user/login", _testsFixture.ConvertToContentInJson(user));

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.GetResponseFromString(responseText);

        Assert.Equal(HttpStatusCode.Forbidden, httpResponse?.StatusCode);
        Assert.Null(response?.Data);
        Assert.True(response?.Errors?.Count > 0);
        Assert.True(response?.Errors?.Any(x => x.ErrorCode == "LockedUser"));
    }

    //[Fact(DisplayName = "ShouldReturnBadRequest_WhenSignInFails")]
    //[Trait("Login User", "")]
    //public async void LoginAsync_ShouldReturnBadRequest_WhenSignInFails()
    //{
    //    // Arrange
    //    var userManagerFake = A.Fake<UserManager<User>>();
    //    var signInManagerFake = A.Fake<SignInManager<User>>();
    //    var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
    //    var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

    //    var requestFake = _testsFixture.GetValidLoginRequestModel();

    //    A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
    //        .Returns(Task.FromResult<User?>(_testsFixture.GetFakeUser()));

    //    A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true))
    //        .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Failed));

    //    // Act
    //    var response = await controller.LoginAsync(requestFake);

    //    // Assert
    //    var objectResult = (ObjectResult?)response;
    //    var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

    //    Assert.Equal((int)HttpStatusCode.BadRequest, objectResult?.StatusCode);
    //    Assert.Null(apiResponse?.Data);
    //    Assert.True(apiResponse?.Errors?.Count > 0);
    //    Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InvalidRequest"));
    //    A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    //    A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    //}

    //[Fact(DisplayName = "ShouldReturnInternalError_WhenNoCryptoKeyAvailable")]
    //[Trait("Login User", "")]
    //public async void LoginAsync_ShouldReturnInternalError_WhenNoCryptoKeyAvailable()
    //{
    //    // Arrange
    //    var userManagerFake = A.Fake<UserManager<User>>();
    //    var signInManagerFake = A.Fake<SignInManager<User>>();
    //    var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
    //    var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake);

    //    var requestFake = _testsFixture.GetValidLoginRequestModel();

    //    A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
    //        .Returns(Task.FromResult<User?>(_testsFixture.GetFakeUser()));

    //    A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true))
    //        .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));

    //    A.CallTo(() => cryptographicKeyRetrieverFake.GetExistingKeyAsync())
    //       .Returns(Task.FromResult<ICryptographicKey?>(null));

    //    // Act
    //    var response = await controller.LoginAsync(requestFake);

    //    // Assert
    //    var objectResult = (ObjectResult?)response;
    //    var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

    //    Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult?.StatusCode);
    //    Assert.Null(apiResponse?.Data);
    //    Assert.True(apiResponse?.Errors?.Count > 0);
    //    Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InternalError"));
    //    A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    //    A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    //}

    //[Fact(DisplayName = "ShouldReturnOkWithToken_WhenRequestIsValid")]
    //[Trait("Login User", "")]
    //public async void LoginAsync_ShouldReturnOkWithToken_WhenRequestIsValid()
    //{
    //    // Arrange
    //    var userManagerFake = A.Fake<UserManager<User>>();
    //    var signInManagerFake = A.Fake<SignInManager<User>>();
    //    var cryptographicKeyRetrieverFake = A.Fake<ICryptographicKeyRetriever>();
    //    var cryptoKeyFake = A.Fake<ICryptographicKey>();
    //    var httpContextFake = A.Fake<HttpContext>();
    //    var requestFake = _testsFixture.GetValidLoginRequestModel();

    //    A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._))
    //        .Returns(Task.FromResult<User?>(_testsFixture.GetFakeUser()));

    //    A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true))
    //        .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));

    //    A.CallTo(() => cryptographicKeyRetrieverFake.GetExistingKeyAsync())
    //       .Returns(Task.FromResult<ICryptographicKey?>(cryptoKeyFake));

    //    A.CallTo(() => cryptoKeyFake.GetSigningCredentials())
    //       .Returns(_testsFixture.GetRsaSigningCredentials());

    //    A.CallTo(() => httpContextFake.Request.Scheme).Returns("https");
    //    A.CallTo(() => httpContextFake.Request.Host).Returns(new HostString("localhost:5000"));

    //    var controller = new AuthController(userManagerFake, signInManagerFake, cryptographicKeyRetrieverFake)
    //    {
    //        ControllerContext = new ControllerContext()
    //        {
    //            HttpContext = httpContextFake
    //        }
    //    };

    //    // Act
    //    var response = await controller.LoginAsync(requestFake);

    //    // Assert
    //    var objectResult = (ObjectResult?)response;
    //    var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);
    //    var accessToken = _testsFixture.GetAccessTokenFromApiResponse(apiResponse);

    //    Assert.Equal((int)HttpStatusCode.OK, objectResult?.StatusCode);
    //    Assert.NotNull(apiResponse?.Data);
    //    Assert.Null(apiResponse?.Errors);
    //    Assert.True(accessToken?.ExpiresInSeconds > 0);
    //    Assert.Equal("at+jwt", accessToken?.TokenType);
    //    Assert.True(!string.IsNullOrWhiteSpace(accessToken?.AccessToken));
    //    A.CallTo(() => userManagerFake.FindByEmailAsync(A<string>._)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    //    A.CallTo(() => signInManagerFake.PasswordSignInAsync(A<string>._, A<string>._, false, true)).MustHaveHappened(numberOfTimes: 1, timesOption: Times.Exactly);
    //}

    [Fact(DisplayName = "ShouldReturnBadRequest_WhenGuidEmpty")]
    [Trait("Get User", "")]
    public async void GetUser_ShouldReturnBadRequest_WhenGuidEmpty()
    {
        //Arrange & Act
        var httpResponse = await _testsFixture.Client.GetAsync($"api/auth/user/{Guid.Empty}");

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.GetResponseFromString(responseText);

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse?.StatusCode);
        Assert.Null(response?.Data);
        Assert.True(response?.Errors?.Count > 0);
        Assert.True(response?.Errors?.Any(x => x.ErrorCode == "InvalidUser"));
    }

    [Fact(DisplayName = "ShouldReturnNotFound_WhenUserNotFound")]
    [Trait("Get User", "")]
    public async void GetUser_ShouldReturnNotFound_WhenUserNotFound()
    {
        //Arrange & Act
        var httpResponse = await _testsFixture.Client.GetAsync($"api/auth/user/{Guid.NewGuid()}");

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.GetResponseFromString(responseText);

        Assert.Equal(HttpStatusCode.NotFound, httpResponse?.StatusCode);
        Assert.Null(response?.Data);
        Assert.True(response?.Errors?.Count > 0);
        Assert.True(response?.Errors?.Any(x => x.ErrorCode == "UserNotFound"));
    }

    [Fact(DisplayName = "ShouldReturnOk_WhenUserFound")]
    [Trait("Get User", "")]
    public async void GetUser_ShouldReturnOk_WhenUserFound()
    {
        //Arrange
        var user = await _testsFixture.RegisterUserAsync();

        //Act
        var httpResponse = await _testsFixture.Client.GetAsync($"api/auth/user/{user.Id}");

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.GetResponseFromString(responseText);
        var data = _testsFixture.GetUserFromAppResponse(response);

        Assert.Equal(HttpStatusCode.OK, httpResponse?.StatusCode);
        Assert.NotNull(response?.Data);
        Assert.Null(response?.Errors?.Count);
        Assert.Equal(user.Id, data?.Id);
        Assert.Equal(user.UserName, data?.UserName);
        Assert.Equal(user.Email, data?.Email);
    }
}