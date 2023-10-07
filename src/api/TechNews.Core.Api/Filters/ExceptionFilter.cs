using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using TechNews.Common.Library.Models;

namespace TechNews.Core.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        Log.Error(context.Exception, context.Exception.Message);

        if (context.Result is not null)
        {
            return;
        }

        context.Result = new ObjectResult(new ApiResponse(error: new ErrorResponse("invalid_request", "InvalidRequest", "There was an unexpected error with the application. Please contact support!")))
        {
            StatusCode = 500
        };
    }
}