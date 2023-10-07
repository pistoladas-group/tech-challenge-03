using Microsoft.AspNetCore.Mvc;

namespace TechNews.Web.Views.Components.Footer;

public class FooterViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}