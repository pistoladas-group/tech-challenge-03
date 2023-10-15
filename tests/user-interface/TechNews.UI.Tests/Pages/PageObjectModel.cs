using TechNews.UI.Tests.Configuration;

namespace TechNews.UI.Tests.Pages;

public abstract class PageObjectModel
{
    protected readonly SeleniumHelper Helper;

    public PageObjectModel(SeleniumHelper helper)
    {
        Helper = helper;
    }

    public string GetCurrentUrl()
    {
        return Helper.GetUrl();
    }
}
