using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TechNews.Common.Library.Models;
using TechNews.Core.Api.Controllers;
using TechNews.Core.Api.Data;
using TechNews.Core.Api.Data.Models;

namespace TechNews.Core.Api.Tests;

public class NewsControllerTests
{
    [Fact]
    public async void GetNewsById_ShouldReturnBadRequest_WhenInvalidIdIsProvided()
    {
        //Arrange
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TechNews")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        using var dbContext = new ApplicationDbContext(contextOptions);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var controller = new NewsController(dbContext);

        //Act
        var response = await controller.GetNewsById(Guid.Empty);

        //Assert
        var objectResult = (ObjectResult?)response;
        Assert.Equal(objectResult?.StatusCode, (int)HttpStatusCode.BadRequest);
        Assert.Null(GetApiResponseFromObjectResult(objectResult)?.Data);
    }

    [Fact]
    public async void GetNewsById_ShouldReturnData_WhenValidIdIsProvided()
    {
        //Arrange
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TechNews")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        using var dbContext = new ApplicationDbContext(contextOptions);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var author = new Author(name: "Everton", email: "everton.teste@gmail.com", imageSource: "https://picsum.photos/200/300");
        var news = new News(title: "Noticinha de Teste", description: "testando notícias", publishDate: DateTime.UtcNow, author: author, imageSource: "https://picsum.photos/200");

        dbContext.News.Add(news);
        dbContext.SaveChanges();

        var controller = new NewsController(dbContext);

        //Act
        var response = await controller.GetNewsById(news.Id);

        //Assert
        var objectResult = (ObjectResult?)response;
        Assert.Equal(objectResult?.StatusCode, (int)HttpStatusCode.OK);
        Assert.NotNull(GetApiResponseFromObjectResult(objectResult)?.Data);
        Assert.Equal(ConvertDataFromObjectResult<News?>(objectResult)?.Id, news.Id);
    }

    private static ApiResponse? GetApiResponseFromObjectResult(ObjectResult? objectResult)
    {
        return (ApiResponse?)objectResult?.Value;
    }

    private static T? ConvertDataFromObjectResult<T>(ObjectResult? objectResult)
    {
        return (T?)GetApiResponseFromObjectResult(objectResult)?.Data;
    }
}