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
        var text = Helper.GetElementTextByXPath("/html/body/app-root/app-identity-root/app-identity-account/div/h1");

        return text.Contains("Cadastro");
    }
}
