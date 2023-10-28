using OpenQA.Selenium;
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
    private const string LoginLinkElementId = "lnkLogin";
    private const string RegisterAccountTitleElementId = "spnRegisterAccountTitle";
    private const string EmailWrapperElementId = "divEmailWrapper";
    private const string AlertValidateClassName = "alert-validate";
    private const string WarningAlertElementId = "divWarningAlert";
    private const string ElementHiddenClassName = "d-none";
    private const string ConfirmPasswordWrapperElementId = "divConfirmPasswordWrapper";

    public RegisterPage(SeleniumHelper helper) : base(helper) { }

    public bool IsPage()
    {
        return Helper.ElementExistsById(PageIdentifierElementId);
    }

    public void FillForm(UserModel user)
    {
        Helper.FillTextInputById(EmailElementId, user.Email);
        Helper.FillTextInputById(UserNameElementId, user.UserName);
        Helper.FillTextInputById(PasswordElementId, user.Password);
        Helper.FillTextInputById(ConfirmPasswordElementId, user.ConfirmPassword);
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

    public void RegisterUser(UserModel user)
    {
        FillForm(user);
        ClickSubmitButton();
    }

    public void ClickLoginLink()
    {
        Helper.ClickElementById(LoginLinkElementId);
    }

    public void ClickRegisterAccountTitle()
    {
        Helper.ClickElementById(RegisterAccountTitleElementId);
    }

    public bool CheckInvalidEmailMessageIsBeingShown()
    {
        return Helper.CheckElementHasClass(By.Id(EmailWrapperElementId), AlertValidateClassName) && 
               Helper.GetElementAttribute(EmailWrapperElementId, "data-validate") == "E-mail inválido";
    }
    
    public bool CheckWeakPasswordMessageIsBeingShown()
    {
        return !Helper.CheckElementHasClass(By.Id(WarningAlertElementId), ElementHiddenClassName) &&
               Helper.GetElementTextById(WarningAlertElementId) == "O campo Senha deve conter pelo menos um digito, uma letra minúscula, uma maiúscula e um caracter especial";
    }
    
    public bool CheckInvalidUserNameMessageIsBeingShown()
    {
        return !Helper.CheckElementHasClass(By.Id(WarningAlertElementId), ElementHiddenClassName) &&
               Helper.GetElementTextById(WarningAlertElementId) == "O campo Nome de Usuário contém caracteres inválidos";
    }

    public bool CheckPasswordDoesntMatchErrorMessageIsBeingShown()
    {
        return Helper.CheckElementHasClass(By.Id(ConfirmPasswordWrapperElementId), AlertValidateClassName) && 
               Helper.GetElementAttribute(ConfirmPasswordWrapperElementId, "data-validate") == "Senhas não conferem";
    }
}
