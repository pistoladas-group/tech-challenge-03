using Bogus;
using TechNews.UI.Tests.Configuration;
using TechNews.UI.Tests.Pages.Models;

namespace TechNews.UI.Tests.Pages;

public class LoginPage : PageObjectModel
{
    private const string PageIdentifierElementId = "login-page";
    private const string EmailElementId = "txtEmail";
    private const string PasswordElementId = "txtPassword";
    private const string LoginUserElementId = "btnLoginUser";
    private const string ErrorAlertElementId = "divErrorAlert";
    private const string WarningAlertElementId = "divWarningAlert";

    public LoginPage(SeleniumHelper helper) : base(helper) { }

    public bool IsPage()
    {
        return Helper.ElementExistsById(PageIdentifierElementId);
    }

    public void FillFormWithUser(UserModel user)
    {
        Helper.FillTextInputById(EmailElementId, user.Email ?? string.Empty);
        Helper.FillTextInputById(PasswordElementId, user.Password ?? string.Empty);
    }

    public void FillFormRandomly()
    {
        Helper.FillTextInputById(EmailElementId, new Faker().Internet.Email());
        Helper.FillTextInputById(PasswordElementId, new Faker().Internet.Password(length: 8, memorable: false, prefix: "1aA@-"));
    }

    public void FillFormWithRandomPassword()
    {
        Helper.FillTextInputById(PasswordElementId, new Faker().Internet.Password(length: 8, memorable: false, prefix: "1aA@-"));
    }

    public void FillFormWithRandomEmail()
    {
        Helper.FillTextInputById(EmailElementId, new Faker().Internet.Email());
    }

    public bool CheckFormIsFilledUp()
    {
        var email = Helper.GetElementValueById(EmailElementId);
        var password = Helper.GetElementValueById(PasswordElementId);

        return
            !string.IsNullOrWhiteSpace(email) &&
            !string.IsNullOrWhiteSpace(password);
    }

    public bool CheckFormIsNotFilledUp()
    {
        var email = Helper.GetElementValueById(EmailElementId);
        var password = Helper.GetElementValueById(PasswordElementId);

        return
            string.IsNullOrWhiteSpace(email) &&
            string.IsNullOrWhiteSpace(password);
    }

    public void ClickSubmitButton()
    {
        Helper.ClickElementById(LoginUserElementId);
    }

    public bool IsErrorShownEqualTo(string errorMessage)
    {
        var value = Helper.GetElementInnerHtmlById(ErrorAlertElementId);

        return value == errorMessage;
    }

    public bool ContainsInShownWarning(string errorMessage)
    {
        var value = Helper.GetElementInnerHtmlById(WarningAlertElementId);

        return value.Contains(errorMessage);
    }
}