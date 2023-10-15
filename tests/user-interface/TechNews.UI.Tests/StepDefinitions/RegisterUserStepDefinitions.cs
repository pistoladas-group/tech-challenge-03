using TechNews.UI.Tests.Fixtures;
using TechNews.UI.Tests.Pages;

namespace TechNews.UI.Tests.StepDefinitions
{
    [Binding]
    [Collection(nameof(TestsFixtureCollection))]
    public sealed class RegisterUserStepDefinitions
    {
        private readonly TestsFixture _fixture;
        private readonly LandingPage _landingPage;

        public RegisterUserStepDefinitions(TestsFixture fixture)
        {
            _fixture = fixture;
            _landingPage = new LandingPage(_fixture.SeleniumHelper);
        }

        [Given(@"the lector is at the landing page")]
        public void GivenTheLectorIsAtTheLandingPage()
        {
            // Arrange & Act
            _landingPage.GoTo();

            // Assert
            Assert.True(_landingPage.IsPage());
        }

        [Given(@"the Register button is clicked")]
        public void GivenTheRegisterButtonIsClicked()
        {
            throw new PendingStepException();
        }

        [When(@"the form is populated correctly")]
        public void WhenTheFormIsPopulatedCorrectly()
        {
            throw new PendingStepException();
        }

        [When(@"the submit button is clicked")]
        public void WhenTheSubmitButtonIsClicked()
        {
            throw new PendingStepException();
        }

        [Then(@"the lector must be logged in")]
        public void ThenTheLectorMustBeLoggedIn()
        {
            throw new PendingStepException();
        }

        [Then(@"the lector must be redirected to the News Home page")]
        public void ThenTheLectorMustBeRedirectedToTheNewsHomePage()
        {
            throw new PendingStepException();
        }

    }
}