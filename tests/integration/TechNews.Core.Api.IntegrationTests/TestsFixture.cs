using System.Text.Json;
using Bogus;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using TechNews.Core.Api.Data;
using TechNews.Core.Api.Data.Models;
using Testcontainers.MsSql;

namespace TechNews.Core.Api.IntegrationTests;

public class TestsFixture : IDisposable, IAsyncLifetime
{
    private CoreApiFactory _coreApiFactory = null!;
    public HttpClient CoreHttpClient = null!;
    
    private MsSqlContainer _sqlServerContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPortBinding(hostPort: 1434, containerPort: 1433)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
        .WithPassword("Pass@123")
        .WithEnvironment(name: "ACCEPT_EULA", value: "Y")
        .WithEnvironment(name: "MSSQL_PID", value: "developer")
        .Build();
    
    public T? Deserialize<T>(string data)
    {
        return string.IsNullOrEmpty(data) 
            ? default 
            : JsonSerializer.Deserialize<T>(data, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }
    
    public Guid AddNewsToDbContext()
    {
        var news = GetNewsWithAuthor();

        using var scope = _coreApiFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext?.News.Add(news);
        dbContext?.SaveChanges();

        return news.Id;
    }

    private News GetNewsWithAuthor()
    {
        var testAuthor = new Faker<Author>()
            .CustomInstantiator(f =>
                new Author(
                    name: f.Name.FirstName(),
                    email: f.Internet.Email(),
                    imageSource: f.Image.PicsumUrl()
                )
            );

        var author = testAuthor.Generate();

        var testNews = new Faker<News>()
            .CustomInstantiator(f =>
                new News(
                    title: string.Join(" ", f.Lorem.Words(f.Random.Number(5, 10))),
                    description: f.Lorem.Paragraphs(),
                    publishDate: f.Date.Recent(),
                    author: author,
                    imageSource: f.Image.PicsumUrl()
                )
            );

        return testNews.Generate();
    }
    
    public void Dispose()
    {
        _coreApiFactory.Dispose();
        CoreHttpClient.Dispose();
    }

    public async Task InitializeAsync()
    {
        await _sqlServerContainer.StartAsync();

        _coreApiFactory = new CoreApiFactory(_sqlServerContainer.GetConnectionString());
        CoreHttpClient = _coreApiFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services => 
                {
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                });
            })  
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
    }

    public async Task DisposeAsync()
    {
        await _sqlServerContainer.StopAsync();
    }
}
