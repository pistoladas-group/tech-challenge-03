using TechNews.UI.Tests.Fixtures;
using TechNews.UI.Tests.Pages;

namespace TechNews.UI.Tests.StepDefinitions;

[Binding]
[Collection(nameof(TestsFixtureCollection))]
public sealed class LoginUserStepDefinitions
{
    private readonly TestsFixture _fixture;
    private readonly LandingPage _landingPage;
    private readonly LoginPage _loginPage;

    public LoginUserStepDefinitions(TestsFixture fixture)
    {
        _fixture = fixture;
        _loginPage = new LoginPage(_fixture.SeleniumHelper);
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

    [When(@"the login form is populated correctly")]
    public void WhenTheLoginFormIsPopulatedCorrectly()
    {
        throw new PendingStepException();
    }

    [When(@"the login submit button is clicked")]
    public void WhenTheLoginSubmitButtonIsClicked()
    {
        throw new PendingStepException();
    }

}