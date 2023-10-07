using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TechNews.Common.Library.Models;

namespace TechNews.Web.Filters;

public class ModelStateFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Result is not null)
        {
            return;
        }

        if (!context.ModelState.IsValid)
        {
            var errors = new List<ErrorAppResponse>();
            foreach (var item in context.ModelState.Values)
            {
                foreach (var error in item.Errors)
                {
                    errors.Add(new ErrorAppResponse("invalid_request", "InvalidRequest", error.ErrorMessage));
                }
            }

            context.Result = new BadRequestObjectResult(new AppResponse() { Errors = errors });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}