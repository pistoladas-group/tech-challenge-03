using Bogus;
using TechNews.UI.Tests.Configuration;
using TechNews.UI.Tests.Pages.Models;

namespace TechNews.UI.Tests.Pages;

public class RegisterPage : PageObjectModel
{
    private const string PageIdentifierElementId = "register-page";
    private const string EmailElementId = "txtEmail";
    private const string UserNameElementId = "txtUsername";
    private const string PasswordElementId = "txtPassword";
    private const string ConfirmPasswordElementId = "txtConfirmPassword";
    private const string RegisterUserElementId = "btnRegisterUser";

    public RegisterPage(SeleniumHelper helper) : base(helper) { }

    public bool IsPage()
    {
        return Helper.ElementExistsById(PageIdentifierElementId);
    }

    public UserModel FillFormCorrectly()
    {
        var user = new UserModel()
        {
            Email = new Faker().Internet.Email(),
            UserName = new Faker().Internet.UserName(),
            Password = new Faker().Internet.Password(length: 8, memorable: false, prefix: "1aA@-")
        };

        Helper.FillTextInputById(EmailElementId, user.Email);
        Helper.FillTextInputById(UserNameElementId, user.UserName);
        Helper.FillTextInputById(PasswordElementId, user.Password);
        Helper.FillTextInputById(ConfirmPasswordElementId, user.Password);

        return user;
    }

    public bool CheckFormIsFilledUp()
    {
        var email = Helper.GetElementValueById(EmailElementId);
        var userName = Helper.GetElementValueById(UserNameElementId);
        var password = Helper.GetElementValueById(PasswordElementId);
        var confirmPassword = Helper.GetElementValueById(ConfirmPasswordElementId);

        return
            !string.IsNullOrEmpty(email) &&
            !string.IsNullOrEmpty(userName) &&
            !string.IsNullOrEmpty(password) &&
            !string.IsNullOrEmpty(confirmPassword);
    }

    public void ClickSubmitButton()
    {
        Helper.ClickElementById(RegisterUserElementId);
    }

    public UserModel RegisterUser()
    {
        var registeredUser = FillFormCorrectly();
        ClickSubmitButton();

        return registeredUser;
    }
}
