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

        Assert.Equal(objectResult?.StatusCode, (int)HttpStatusCode.BadRequest);
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

        Assert.Equal(objectResult?.StatusCode, (int)HttpStatusCode.OK);
        Assert.NotNull(apiResponse?.Data);
        Assert.Equal(apiResponseData?.Id, newsId);
    }
}