using Microsoft.AspNetCore.Mvc;
using TechNews.Web.Models;

namespace TechNews.Web.Views.Shared.Components.NavigationBar;

public class NavigationBarViewComponent : ViewComponent
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public NavigationBarViewComponent(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IViewComponentResult Invoke()
    {
        var model = new UserModel
        {
            IsAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false
        };

        if (model.IsAuthenticated)
        {
            model.UserName = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type.Equals("name", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
        }

        return View(model);
    }
}