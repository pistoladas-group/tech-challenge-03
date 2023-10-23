using TechNews.UI.Tests.Configuration;

namespace TechNews.UI.Tests.Pages;

public class NewsPage : PageObjectModel
{
    private const string PageIdentifierElementId = "news-page";

    public NewsPage(SeleniumHelper helper) : base(helper) { }

    public bool IsPage()
    {
        return Helper.ElementExistsById(PageIdentifierElementId);
    }
}
