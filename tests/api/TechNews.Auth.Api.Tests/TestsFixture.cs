using Bogus;
using Microsoft.AspNetCore.Mvc;
using TechNews.Auth.Api.Models;
using TechNews.Common.Library.Models;

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

    public ApiResponse? GetApiResponseFromObjectResult(ObjectResult? objectResult)
    {
        return (ApiResponse?)objectResult?.Value;
    }

    public T? ConvertDataFromObjectResult<T>(ObjectResult? objectResult)
    {
        return (T?)GetApiResponseFromObjectResult(objectResult)?.Data;
    }

    public void Dispose()
    {
    }
}