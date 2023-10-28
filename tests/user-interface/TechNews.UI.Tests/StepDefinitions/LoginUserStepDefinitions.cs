using TechNews.UI.Tests.Fixtures;
using TechNews.UI.Tests.Pages;
using TechNews.UI.Tests.Pages.Models;

namespace TechNews.UI.Tests.StepDefinitions;

[Binding]
[Collection(nameof(TestsFixtureCollection))]
public sealed class LoginUserStepDefinitions
{
    private readonly TestsFixture _fixture;
    private readonly LandingPage _landingPage;
    private readonly RegisterPage _registerPage;
    private readonly NavigationPage _navigationPage;
    private readonly LoginPage _loginPage;

    public LoginUserStepDefinitions(TestsFixture fixture)
    {
        _fixture = fixture;
        _loginPage = new LoginPage(fixture.SeleniumHelper);
        _registerPage = new RegisterPage(fixture.SeleniumHelper);
        _navigationPage = new NavigationPage(fixture.SeleniumHelper);
        _landingPage = new LandingPage(fixture.SeleniumHelper);
    }

    [Given(@"the login button is clicked")]
    public void GivenTheLoginButtonIsClicked()
    {
        // Arrange & Act
        _landingPage.ClickLoginButton();

        // Assert
        Assert.True(_loginPage.IsPage());
    }

    [Given(@"the login form is populated with a registered lector")]
    public void WhenTheLoginFormIsPopulatedWithARegisteredLector()
    {
        // Arrange
        var registeredUser = RegisterUserAndGoToLoginPage();

        // Act
        _loginPage.FillFormWithUser(registeredUser);

        // Assert
        Assert.True(_loginPage.CheckFormIsFilledUp());
    }

    [Given(@"the login form is populated with an unexistent lector")]
    public void WhenTheLoginFormIsPopulatedWithAnUnexistentLector()
    {
        // Arrange & Act
        _loginPage.FillFormRandomly();

        // Assert
        Assert.True(_loginPage.CheckFormIsFilledUp());
    }

    [Given(@"the password is incorrect")]
    public void GivenThePasswordIsIncorrect()
    {
        // Arrange & Act
        _loginPage.FillFormWithRandomPassword();

        // Assert
        Assert.True(_loginPage.CheckFormIsFilledUp());
    }

    [Given(@"the email is incorrect")]
    public void GivenTheEmailIsIncorrect()
    {
        // Arrange & Act
        _loginPage.FillFormWithRandomPassword();

        // Assert
        Assert.True(_loginPage.CheckFormIsFilledUp());
    }

    [Given(@"the login form is not populated")]
    public void GivenTheLoginFormIsNotPopulated()
    {
        // Arrange & Act & Assert
        Assert.True(_loginPage.CheckFormIsNotFilledUp());
    }


    [When(@"the login submit button is clicked")]
    public void WhenTheLoginSubmitButtonIsClicked()
    {
        // Arrange & Act & Assert
        _loginPage.ClickSubmitButton();
    }

    [When(@"the login button is clicked")]
    public void WhenTheLoginButtonIsClicked()
    {
        // Arrange & Act & Assert
        _landingPage.ClickLoginButton();
    }

    [When(@"the login link is clicked")]
    public void WhenTheLoginLinkIsClicked()
    {
        // Arrange & Act & Assert
        _registerPage.ClickLoginLink();
    }

    [When(@"the login submit button is clicked more than three times")]
    public void WhenTheLoginSubmitButtonIsClickedMoreThanThreeTimes()
    {
        // Arrange & Act & Assert
        _loginPage.ClickSubmitButton();
        Thread.Sleep(TimeSpan.FromSeconds(1));
        _loginPage.ClickSubmitButton();
        Thread.Sleep(TimeSpan.FromSeconds(1));
        _loginPage.ClickSubmitButton();
    }


    [Then(@"a generic error message appears")]
    public void ThenAGenericErrorMessageAppears()
    {
        // Arrange & Act & Assert
        _loginPage.IsErrorShownEqualTo("Usuário ou senha inválidos");
    }

    [Then(@"the lector must be redirected to the login page")]
    public void ThenTheLectorMustBeRedirectedToTheLoginPage()
    {
        // Arrange & Act & Assert
        Assert.True(_loginPage.IsPage());
    }

    [Then(@"a lockout message appears")]
    public void ThenALockoutMessageAppears()
    {
        // Arrange & Act & Assert
        _loginPage.ContainsInShownWarning("Número máximo de tentativas de login excedido");
    }

    [Then(@"the user remains in the login page")]
    public void ThenTheUserRemainsInTheLoginPage()
    {
        // Assert & Act & Assert
        Assert.True(_loginPage.IsPage());
    }

    private UserModel RegisterUserAndGoToLoginPage()
    {
        _landingPage.GoTo();
        _landingPage.ClickRegisterButton();
        
        var user = _fixture.GetValidUser();
        
        _registerPage.RegisterUser(user);
        _navigationPage.ClickLogoutButton();
        _landingPage.GoTo();
        _landingPage.ClickLoginButton();

        return user;
    }
}