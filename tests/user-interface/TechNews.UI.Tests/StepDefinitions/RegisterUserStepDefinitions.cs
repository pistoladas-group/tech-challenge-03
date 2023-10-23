using TechNews.UI.Tests.Fixtures;
using TechNews.UI.Tests.Pages;

namespace TechNews.UI.Tests.StepDefinitions;

[Binding]
[Collection(nameof(TestsFixtureCollection))]
public sealed class RegisterUserStepDefinitions
{
    private readonly TestsFixture _fixture;
    private readonly LandingPage _landingPage;
    private readonly RegisterPage _registerPage;

    public RegisterUserStepDefinitions(TestsFixture fixture)
    {
        _fixture = fixture;
        _registerPage = new RegisterPage(_fixture.SeleniumHelper);
        _landingPage = new LandingPage(_fixture.SeleniumHelper);
    }

    [Given(@"the register button is clicked")]
    public void GivenTheRegisterButtonIsClicked()
    {
        // Arrange & Act
        _landingPage.ClickRegisterButton();

        // Assert
        Assert.True(_registerPage.IsPage());
    }

    [When(@"the register form is populated correctly")]
    public void WhenTheRegisterFormIsPopulatedCorrectly()
    {
        // Arrange & Act
        _registerPage.FillFormCorrectly();

        // Assert
        Assert.True(_registerPage.CheckFormIsFilledUp());
    }

    [When(@"the register submit button is clicked")]
    public void WhenTheRegisterSubmitButtonIsClicked()
    {
        throw new PendingStepException();
    }
}