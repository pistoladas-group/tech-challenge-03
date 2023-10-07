using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechNews.Web.Models;

namespace TechNews.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    [AllowAnonymous]
    public IActionResult Index()
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
