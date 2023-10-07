using Microsoft.AspNetCore.Mvc;
using TechNews.Web.Models;

namespace TechNews.Web.Views.Components.Post;

public class PostViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(News news)
    {
        return View(news);
    }
}