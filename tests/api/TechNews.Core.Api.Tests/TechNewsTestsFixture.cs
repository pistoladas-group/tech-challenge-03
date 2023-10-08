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

    public Guid AddNewsToDbContext()
    {
        var author = new Author(name: "Everton", email: "everton.teste@gmail.com", imageSource: "https://picsum.photos/200/300");
        var news = new News(title: "Noticinha de Teste", description: "testando notícias", publishDate: DateTime.UtcNow, author: author, imageSource: "https://picsum.photos/200");

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