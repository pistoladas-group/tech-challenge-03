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
        _loginPage = new LoginPage(_fixture.SeleniumHelper);
        _registerPage = new RegisterPage(_fixture.SeleniumHelper);
        _navigationPage = new NavigationPage(_fixture.SeleniumHelper);
        _landingPage = new LandingPage(_fixture.SeleniumHelper);
    }

    [Given(@"the login button is clicked")]
    public void GivenTheLoginButtonIsClicked()
    {
        // Arrange & Act
        _landingPage.ClickLoginButton();

        // Assert
        Assert.True(_loginPage.IsPage());
    }

    [When(@"the login form is populated with a registered lector")]
    public void WhenTheLoginFormIsPopulatedWithARegisteredLector()
    {
        // Arrange
        var registeredUser = RegisterUserAndGoToLoginPage();

        // Act
        _loginPage.FillFormCorrectly(registeredUser);

        // Assert
        Assert.True(_loginPage.CheckFormIsFilledUp());
    }

    [When(@"the login submit button is clicked")]
    public void WhenTheLoginSubmitButtonIsClicked()
    {
        // Arrange & Act & Assert
        _loginPage.ClickSubmitButton();
    }

    private UserModel RegisterUserAndGoToLoginPage()
    {
        _landingPage.GoTo();
        _landingPage.ClickRegisterButton();
        var registeredUser = _registerPage.RegisterUser();
        _navigationPage.ClickLogoutButton();
        _landingPage.GoTo();
        _landingPage.ClickLoginButton();

        return registeredUser;
    }
}