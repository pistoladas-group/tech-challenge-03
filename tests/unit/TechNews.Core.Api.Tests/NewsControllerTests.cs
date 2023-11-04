using System.Net;
using Microsoft.AspNetCore.Mvc;
using TechNews.Core.Api.Controllers;
using TechNews.Core.Api.Data.Models;

namespace TechNews.Core.Api.Tests;

public class NewsControllerTests : IClassFixture<TestsFixture>
{
    private TestsFixture _testsFixture { get; set; }

    public NewsControllerTests(TestsFixture testsFixture)
    {
        _testsFixture = testsFixture;
    }

    [Fact]
    public async void GetNewsById_ShouldReturnBadRequest_WhenInvalidIdIsProvided()
    {
        //Arrange
        var dbContext = _testsFixture.GetDbContext();
        var controller = new NewsController(dbContext);

        //Act
        var response = await controller.GetNewsById(Guid.Empty);

        //Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal((int)HttpStatusCode.BadRequest, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
    }

    [Fact]
    public async void GetNewsById_ShouldReturnOkWithData_WhenValidIdIsProvided()
    {
        //Arrange
        var dbContext = _testsFixture.GetDbContext();
        var controller = new NewsController(dbContext);

        var newsId = _testsFixture.AddNewsToDbContext();

        //Act
        var response = await controller.GetNewsById(newsId);

        //Assert
        var objectResult = (ObjectResult?)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);
        var apiResponseData = _testsFixture.ConvertDataFromObjectResult<News?>(objectResult);

        Assert.Equal((int)HttpStatusCode.OK, objectResult?.StatusCode);
        Assert.NotNull(apiResponse?.Data);
        Assert.Equal(newsId, apiResponseData?.Id);
    }

    [Fact]
    public async void GetNewsById_ShouldReturnNotFound_WhenNewsDoesNotExists()
    {
        //Arrange
        var dbContext = _testsFixture.GetDbContext();
        var controller = new NewsController(dbContext);

        //Act
        var response = await controller.GetNewsById(Guid.NewGuid());

        //Assert
        var objectResult = (ObjectResult)response;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(objectResult);

        Assert.Equal((int)HttpStatusCode.NotFound, objectResult?.StatusCode);
        Assert.Null(apiResponse?.Data);
    }

    [Fact]
    public async void GetAllNews_ShouldReturnOk()
    {
        //Arrange
        var dbContext = _testsFixture.GetDbContext();
        var controller = new NewsController(dbContext);
        _testsFixture.AddNewsToDbContext();

        //Act
        var response = await controller.GetAllNewsAsync();

        //Assert
        var objectResult = (ObjectResult?)response;
        var apiResponseData = _testsFixture.ConvertDataFromObjectResult<List<News>?>(objectResult);

        Assert.Equal((int)HttpStatusCode.OK, objectResult?.StatusCode);
        Assert.NotNull(apiResponseData);
    }
}