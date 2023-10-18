﻿using Bogus;
using TechNews.UI.Tests.Configuration;

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

    public void FillFormCorrectly()
    {
        var email = new Faker().Internet.Email();
        var userName = new Faker().Internet.UserName();
        var validPassword = new Faker().Internet.Password(length: 8, memorable: false, prefix: "1aA@-");

        Helper.FillTextInputById(EmailElementId, email);
        Helper.FillTextInputById(UserNameElementId, userName);
        Helper.FillTextInputById(PasswordElementId, validPassword);
        Helper.FillTextInputById(ConfirmPasswordElementId, validPassword);
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
}
