using Bogus;
using TechNews.UI.Tests.Configuration;

namespace TechNews.UI.Tests.Pages;

public class LoginPage : PageObjectModel
{
    private const string PageIdentifierElementId = "login-page";

    public LoginPage(SeleniumHelper helper) : base(helper) { }

    public bool IsPage()
    {
        return Helper.ElementExistsById(PageIdentifierElementId);
    }
}