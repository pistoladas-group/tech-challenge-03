using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using FakeItEasy;
using TechNews.Auth.Api.Filters;

namespace TechNews.Auth.Api.Tests;

public class ExceptionFilterTests : IClassFixture<TestsFixture>
{
    private TestsFixture _testsFixture { get; set; }

    public ExceptionFilterTests(TestsFixture testsFixture)
    {
        _testsFixture = testsFixture;
    }

    [Fact(DisplayName = "ShouldReturn_WhenContextResultNotNull")]
    [Trait("On Exception", "")]
    public void OnException_ShouldReturnResult_WhenContextResultNotNull()
    {
        // Arrange
        var httpContext = A.Fake<HttpContext>();
        var request = A.Fake<HttpRequest>();
        var response = A.Fake<HttpResponse>();

        // Configure the fake HttpContext
        A.CallTo(() => httpContext.Request).Returns(request);
        A.CallTo(() => httpContext.Response).Returns(response);

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var filters = new List<IFilterMetadata>();

        var exceptionContext = new ExceptionContext(actionContext, filters);
        exceptionContext.Exception = new NotImplementedException();
        exceptionContext.Result = new ObjectResult("Custom error message")
        {
            StatusCode = StatusCodes.Status418ImATeapot
        };

        var filter = new ExceptionFilter();

        // Act
        filter.OnException(exceptionContext);

        var teste = (ObjectResult)exceptionContext.Result;

        // Assert
        Assert.NotNull(exceptionContext.Result);
        Assert.Equal("Custom error message", teste.Value);
        Assert.Equal(418, teste.StatusCode);
    }

    [Fact(DisplayName = "ShouldReturnInternalErrorResult_WhenContextResultNull")]
    [Trait("On Exception", "")]
    public void OnException_ShouldReturnInternalErrorResult_WhenContextResultNull()
    {
        // Arrange
        var httpContext = A.Fake<HttpContext>();
        var request = A.Fake<HttpRequest>();
        var response = A.Fake<HttpResponse>();

        // Configure the fake HttpContext
        A.CallTo(() => httpContext.Request).Returns(request);
        A.CallTo(() => httpContext.Response).Returns(response);

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var filters = new List<IFilterMetadata>();

        var exceptionContext = new ExceptionContext(actionContext, filters);
        exceptionContext.Exception = new NotImplementedException();

        var filter = new ExceptionFilter();

        // Act
        filter.OnException(exceptionContext);

        // Assert
        var resultAfterAct = (ObjectResult?)exceptionContext.Result;
        var apiResponse = _testsFixture.GetApiResponseFromObjectResult(resultAfterAct);

        Assert.NotNull(exceptionContext.Result);
        Assert.Equal(500, resultAfterAct?.StatusCode);
        Assert.Null(apiResponse?.Data);
        Assert.True(apiResponse?.Errors?.Count > 0);
        Assert.True(apiResponse?.Errors?.Any(x => x.ErrorCode == "InternalError"));
    }
}