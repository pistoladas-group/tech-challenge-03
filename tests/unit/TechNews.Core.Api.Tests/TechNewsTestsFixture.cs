using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TechNews.Common.Library.Models;
using TechNews.Core.Api.Data;
using TechNews.Core.Api.Data.Models;

namespace TechNews.Core.Api.Tests;

[CollectionDefinition(nameof(TestsFixtureCollection))]
public class TestsFixtureCollection : ICollectionFixture<TestsFixture>
{
}

public class TestsFixture : IDisposable
{
    private ApplicationDbContext? _applicationDbContext { get; set; }
    
    public ApplicationDbContext GetDbContext()
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TechNews")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _applicationDbContext = new ApplicationDbContext(contextOptions);

        _applicationDbContext.Database.EnsureDeleted();
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
}