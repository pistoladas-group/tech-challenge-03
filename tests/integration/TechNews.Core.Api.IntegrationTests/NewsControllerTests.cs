using System.Net;
using TechNews.Common.Library.Models;
using TechNews.Core.Api.Data.Models;

namespace TechNews.Core.Api.IntegrationTests;

public class NewsControllerTests : IClassFixture<TestsFixture>
{
    private readonly TestsFixture _testsFixture;

    public NewsControllerTests(TestsFixture testsFixture)
    {
        _testsFixture = testsFixture;
    }
    
    [Fact(DisplayName = "ShouldReturnOk")]
    [Trait("Get All News", "")]
    public async void GetAllNewsAsync_ShouldReturnOk()
    {
        // Arrange & Act
        var httpResponse = await _testsFixture.CoreHttpClient.GetAsync("api/news");

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.Deserialize<AppResponse>(responseText);

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(response?.Data);
        Assert.Null(response?.Errors?.Count);
    }
    
    [Fact(DisplayName = "ShouldReturnBadRequest_WhenInvalidIdIsProvided")]
    [Trait("Get News By Id", "")]
    public async void GetNewsById_ShouldReturnBadRequest_WhenInvalidIdIsProvided()
    {
        // Arrange & Act
        var httpResponse = await _testsFixture.CoreHttpClient.GetAsync($"api/news/{Guid.Empty}");

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.Deserialize<AppResponse>(responseText);

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.Null(response?.Data);
        Assert.True(response?.Errors?.Count > 0);
        Assert.True(response?.Errors?.Any(x => x.ErrorCode == "InvalidNewsId"));
    }
    
    [Fact(DisplayName = "ShouldReturnNotFound_WhenNoNewsIsFoundWithGivenId")]
    [Trait("Get News By Id", "")]
    public async void GetNewsById_ShouldReturnNotFound_WhenNoNewsIsFoundWithGivenId()
    {
        // Arrange & Act
        var httpResponse = await _testsFixture.CoreHttpClient.GetAsync($"api/news/{Guid.NewGuid()}");

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.Deserialize<AppResponse>(responseText);

        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        Assert.Null(response?.Data);
        Assert.True(response?.Errors?.Count > 0);
        Assert.True(response?.Errors?.Any(x => x.ErrorCode == "NewsNotFound"));
    }
    
    [Fact(DisplayName = "ShouldReturnOk_WhenValidNewsIdIsProvided")]
    [Trait("Get News By Id", "")]
    public async void GetNewsById_ShouldReturnOk_WhenValidNewsIdIsProvided()
    {
        // Arrange
        var newsId = _testsFixture.AddNewsToDbContext();
        
        // Act
        var httpResponse = await _testsFixture.CoreHttpClient.GetAsync($"api/news/{newsId}");

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.Deserialize<AppResponse>(responseText);
        var responseData = _testsFixture.Deserialize<News>(response?.Data?.ToString());

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(response?.Data);
        Assert.Null(response?.Errors?.Count);
        Assert.Equal(newsId, responseData?.Id);
    }
}