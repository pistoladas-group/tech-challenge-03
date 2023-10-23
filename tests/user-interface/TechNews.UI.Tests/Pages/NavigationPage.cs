using TechNews.UI.Tests.Configuration;

namespace TechNews.UI.Tests.Pages;

public class NavigationPage : PageObjectModel
{
    private const string LogoutElementId = "btnLogout";
    private const string LoggedUserNameElementId = "txtLoggedUserName";

    public NavigationPage(SeleniumHelper helper) : base(helper) { }

    public void ClickLogoutButton()
    {
        Helper.ClickElementById(LogoutElementId);
    }

    public bool IsUserLoggedIn()
    {
        var value = Helper.GetElementInnerHtmlById(LoggedUserNameElementId);

        return value.Contains("Olá, ");
    }
}
