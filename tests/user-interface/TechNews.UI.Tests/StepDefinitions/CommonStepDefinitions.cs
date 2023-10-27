using TechNews.UI.Tests.Fixtures;
using TechNews.UI.Tests.Pages;

namespace TechNews.UI.Tests.StepDefinitions;

[Binding]
[Collection(nameof(TestsFixtureCollection))]
public sealed class CommonStepDefinitions
{
    private readonly LandingPage _landingPage;
    private readonly NavigationPage _navigationPage;
    private readonly NewsPage _newsPage;

    public CommonStepDefinitions(TestsFixture fixture)
    {
        _landingPage = new LandingPage(fixture.SeleniumHelper);
        _navigationPage = new NavigationPage(fixture.SeleniumHelper);
        _newsPage = new NewsPage(fixture.SeleniumHelper);
    }

    [Given(@"the lector is at the landing page")]
    public void GivenTheLectorIsAtTheLandingPage()
    {
        // Arrange & Act
        _landingPage.GoTo();

        // Assert
        Assert.True(_landingPage.IsPage());
    }

    [Then(@"the lector must be logged in")]
    public void ThenTheLectorMustBeLoggedIn()
    {
        // Arrange & Act & Assert
        Assert.True(_navigationPage.IsUserLoggedIn());
    }

    [Then(@"the lector must be redirected to the News Home page")]
    public void ThenTheLectorMustBeRedirectedToTheNewsHomePage()
    {
        // Arrange & Act & Assert
        Assert.True(_newsPage.IsPage());
    }
}