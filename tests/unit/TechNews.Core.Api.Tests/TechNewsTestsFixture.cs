using Bogus;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechNews.Common.Library.Models;
using TechNews.Core.Api.Data;
using TechNews.Core.Api.Data.Models;
using Testcontainers.MsSql;

namespace TechNews.Core.Api.Tests;

[CollectionDefinition(nameof(TestsFixtureCollection))]
public class TestsFixtureCollection : ICollectionFixture<TestsFixture>
{
}

public class TestsFixture : IDisposable, IAsyncLifetime
{
    private ApplicationDbContext? _applicationDbContext { get; set; }
    private MsSqlContainer _sqlServerContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPortBinding(hostPort: 1434, containerPort: 1433)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
        .WithPassword("Pass@123")
        .WithEnvironment(name: "ACCEPT_EULA", value: "Y")
        .WithEnvironment(name: "MSSQL_PID", value: "developer")
        .Build();
    
    public ApplicationDbContext GetDbContext()
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_sqlServerContainer.GetConnectionString())
            .Options;

        _applicationDbContext = new ApplicationDbContext(contextOptions);

        _applicationDbContext.Database.EnsureCreated();

        return _applicationDbContext;
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

    public Guid AddNewsToDbContext()
    {
        var news = GetNewsWithAuthor();

        _applicationDbContext?.News.Add(news);
        _applicationDbContext?.SaveChanges();

        return news.Id;
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
        _applicationDbContext?.Dispose();
    }

    public async Task InitializeAsync()
    {
        await _sqlServerContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _sqlServerContainer.StopAsync();
    }
}