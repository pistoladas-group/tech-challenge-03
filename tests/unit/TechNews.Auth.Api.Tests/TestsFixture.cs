using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Bogus;
using TechNews.Auth.Api.Models;
using TechNews.Common.Library.Models;
using TechNews.Auth.Api.Data;

namespace TechNews.Core.Api.Tests;

[CollectionDefinition(nameof(TestsFixtureCollection))]
public class TestsFixtureCollection : ICollectionFixture<TestsFixture>
{
}

public class TestsFixture : IDisposable
{
    public RegisterUserRequestModel GetValidRegisterUserRequestModel()
    {
        var validPassword = new Faker().Internet.Password(length: 8, memorable: false, prefix: @"1aA@-");

        var requestFake = new Faker<RegisterUserRequestModel>()
            .CustomInstantiator(f =>
                new RegisterUserRequestModel()
                {
                    Id = Guid.NewGuid(),
                    Email = f.Internet.Email(),
                    UserName = f.Internet.UserName(),
                    Password = validPassword,
                    Repassword = validPassword
                }
            );

        return requestFake.Generate();
    }

    public LoginRequestModel GetValidLoginRequestModel()
    {
        var requestFake = new Faker<LoginRequestModel>()
            .CustomInstantiator(f =>
                new LoginRequestModel()
                {
                    Email = f.Internet.Email(),
                    Password = new Faker().Internet.Password(length: 8, memorable: false, prefix: @"1aA@-")
                }
            );

        return requestFake.Generate();
    }

    public ApiResponse? GetApiResponseFromObjectResult(ObjectResult? objectResult)
    {
        return (ApiResponse?)objectResult?.Value;
    }

    public IdentityResult GetDefaultIdentityFailure()
    {
        return IdentityResult.Failed(new IdentityError()
        {
            Code = "DefaultError",
            Description = "An unexpected error occurred while creating the user."
        });
    }

    public SigningCredentials GetRsaSigningCredentials()
    {
        var key = RSA.Create(2048);

        return new SigningCredentials(new RsaSecurityKey(key), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };
    }

    public AccessTokenResponse? GetAccessTokenFromApiResponse(ApiResponse? apiResponse)
    {
        if (apiResponse is null)
        {
            return null;
        }

        if (apiResponse.Data is null)
        {
            return null;
        }

        return (AccessTokenResponse)apiResponse.Data;
    }

    public User GetFakeUser()
    {
        var userFake = new Faker<User>()
            .CustomInstantiator(f =>
                new User(Guid.NewGuid(), f.Internet.Email(), f.Internet.UserName())
            );

        return userFake.Generate();
    }

    public IList<Claim> GetFakeClaims()
    {
        return new List<Claim>()
        {
            { new Claim("claim1", "testClaim1") },
            { new Claim("claim2", "testClaim2") }
        };
    }

    public IList<string> GetFakeRoles()
    {
        return new List<string>()
        {
            { "testRole1" },
            { "testRole2" }
        };
    }

    public T? ConvertDataFromObjectResult<T>(ObjectResult? objectResult)
    {
        return (T?)GetApiResponseFromObjectResult(objectResult)?.Data;
    }

    public void Dispose()
    {
    }
}