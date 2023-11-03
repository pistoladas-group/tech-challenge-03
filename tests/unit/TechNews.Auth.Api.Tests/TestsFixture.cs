using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Bogus;
using FakeItEasy;
using TechNews.Auth.Api.Models;
using TechNews.Common.Library.Models;
using TechNews.Auth.Api.Data;
using TechNews.Auth.Api.Services.Cryptography;

namespace TechNews.Auth.Api.Tests;

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
                    Password = f.Internet.Password(length: 8, memorable: false, prefix: @"1aA@-")
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

    public List<ICryptographicKey> GetFakeJwksKeys()
    {
        var fakeKey = A.Fake<ICryptographicKey>();

        A.CallTo(() => fakeKey.GetJsonWebKey())
        .Returns(new JsonWebKeyModel()
        {
            KeyType = "RSA",
            KeyId = Guid.NewGuid().ToString(),
            Algorithm = "RS256",
            Use = "sig",
            Modulus = "AQAB",
            Exponent = "0quLYDiZIxssFKreHcXeeUIbgyU-dctbQXTfBTbAKp4Jl_TH-FQt3EfBVbo2P_1bkH-6ofvDSkQDUbigOhN4zx7JwbjAl8P18-dgjxuhF9HRdZA2W54VxBspEuHhqpsFZKoH_409ywbnc0DtAT-OQR3oQ-6ZnJfUOkLvw7o62QSDyscEi_zh8NIAGQnBo98UVVWr6lbR_PIm7l_NZu0LAux-P5Av-CxAxf32Dvl6crfv_I8ME3_fRisfKaVn5qOt_XuSXmygtTtT94lwelCCuutT6VjjIe397j83yR6LDZACOY7aAw8dx_rb3TS-SgvxQoBshj3142B4RFTVwupyQQ"
        });

        var fakeKeys = new List<ICryptographicKey>();

        fakeKeys.Add(fakeKey);

        return fakeKeys;
    }

    public T? ConvertDataFromObjectResult<T>(ObjectResult? objectResult)
    {
        return (T?)GetApiResponseFromObjectResult(objectResult)?.Data;
    }

    public void Dispose()
    {
    }
}