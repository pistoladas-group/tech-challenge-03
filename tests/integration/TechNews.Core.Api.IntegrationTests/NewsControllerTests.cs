using System.Net;
using TechNews.Common.Library.Models;

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
        //Arrange & Act
        var httpResponse = await _testsFixture.CoreHttpClient.GetAsync("api/news");

        // Assert
        var responseText = await httpResponse.Content.ReadAsStringAsync();
        var response = _testsFixture.Deserialize<AppResponse>(responseText);

        Assert.Equal(HttpStatusCode.OK, httpResponse?.StatusCode);
        Assert.NotNull(response?.Data);
        Assert.Null(response?.Errors?.Count);
    }
}