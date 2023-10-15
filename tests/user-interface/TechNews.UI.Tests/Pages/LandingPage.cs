using TechNews.UI.Tests.Configuration;

namespace TechNews.UI.Tests.Pages;

public class LandingPage : PageObjectModel
{
    public LandingPage(SeleniumHelper helper) : base(helper) { }

    public void GoTo()
    {
        Helper.GoToUrl("");
    }

    public bool IsPage()
    {
        return Helper.ElementExistsById("landing-page");
    }
}
