using TechNews.UI.Tests.Configuration;
using TechNews.UI.Tests.Pages.Models;

namespace TechNews.UI.Tests.Pages;

public class LoginPage : PageObjectModel
{
    private const string PageIdentifierElementId = "login-page";
    private const string EmailElementId = "txtEmail";
    private const string PasswordElementId = "txtPassword";
    private const string LoginUserElementId = "btnLoginUser";

    public LoginPage(SeleniumHelper helper) : base(helper) { }

    public bool IsPage()
    {
        return Helper.ElementExistsById(PageIdentifierElementId);
    }

    public void FillFormCorrectly(UserModel user)
    {
        Helper.FillTextInputById(EmailElementId, user.Email ?? string.Empty);
        Helper.FillTextInputById(PasswordElementId, user.Password ?? string.Empty);
    }

    public bool CheckFormIsFilledUp()
    {
        var email = Helper.GetElementValueById(EmailElementId);
        var password = Helper.GetElementValueById(PasswordElementId);

        return
            !string.IsNullOrWhiteSpace(email) &&
            !string.IsNullOrWhiteSpace(password);
    }

    public void ClickSubmitButton()
    {
        Helper.ClickElementById(LoginUserElementId);
    }
}