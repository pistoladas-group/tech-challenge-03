using TechNews.UI.Tests.Configuration;

namespace TechNews.UI.Tests.Pages;

public class LandingPage : PageObjectModel
{
    private const string PageIdentifierElementId = "landing-page";
    private const string RegisterUserElementId = "btnRegisterUser";

    public LandingPage(SeleniumHelper helper) : base(helper) { }

    public void GoTo()
    {
        Helper.GoToUrl("");
    }

    public bool IsPage()
    {
        return Helper.ElementExistsById(PageIdentifierElementId);
    }

    public void ClickRegisterButton()
    {
        Helper.ClickElementById(RegisterUserElementId);
    }
}
