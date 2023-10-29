using System.Text;
using System.Text.Json;
using Bogus;
using TechNews.Auth.Api.Models;
using TechNews.Common.Library.Models;

namespace TechNews.Auth.Api.IntegrationTests;

[CollectionDefinition(nameof(TestsFixtureCollection))]
public class TestsFixtureCollection : ICollectionFixture<TestsFixture<Program>>
{
}

public class TestsFixture<TProgram> : IDisposable where TProgram : class
{
    public readonly AuthApiFactory<TProgram> Factory;
    public HttpClient Client;

    public TestsFixture()
    {
        Factory = new AuthApiFactory<TProgram>();
        Client = Factory.CreateClient();
    }

    public HttpContent ConvertToContentInJson(object objectToConvert)
    {
        return new StringContent(JsonSerializer.Serialize(objectToConvert), Encoding.UTF8, "application/json");
    }

    public AppResponse? GetResponseFromString(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return JsonSerializer.Deserialize<AppResponse>(text, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }

    public AccessTokenResponse? GetAccessTokenFromAppResponse(AppResponse? appResponse)
    {
        if (appResponse is null)
        {
            return null;
        }

        if (appResponse.Data is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<AccessTokenResponse>(appResponse.Data.ToString(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }

    public GetUserResponseModel? GetUserFromAppResponse(AppResponse? appResponse)
    {
        if (appResponse is null)
        {
            return null;
        }

        if (appResponse.Data is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<GetUserResponseModel>(appResponse.Data.ToString(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }

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

    public async Task<RegisterUserRequestModel> RegisterUserAsync()
    {
        var user = GetValidRegisterUserRequestModel();
        var httpResponse = await Client.PostAsync("api/auth/user", ConvertToContentInJson(user));

        httpResponse.EnsureSuccessStatusCode();

        return user;
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

    public async Task AttemptLoginAsync(RegisterUserRequestModel user, int forManyTimes = 1)
    {
        for (int i = 0; i < forManyTimes; i++)
        {
            await Client.PostAsync("api/auth/user/login", ConvertToContentInJson(user));
        }
    }

    public void Dispose()
    {
        Factory.Dispose();
        Client.Dispose();
    }
}
