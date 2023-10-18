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
        private readonly RegisterPage _registerPage;

        public RegisterUserStepDefinitions(TestsFixture fixture)
        {
            _fixture = fixture;
            _registerPage = new RegisterPage(_fixture.SeleniumHelper);
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
            // Arrange & Act
            _landingPage.ClickRegisterButton();

            // Assert
            Assert.True(_registerPage.IsPage());
        }

        [When(@"the form is populated correctly")]
        public void WhenTheFormIsPopulatedCorrectly()
        {
            // Arrange & Act
            _registerPage.FillFormCorrectly();

            // Assert
            Assert.True(_registerPage.CheckFormIsFilledUp());
        }

        [When(@"the submit button is clicked")]
        public void WhenTheSubmitButtonIsClicked()
        {
            // Arrange & Act
            _registerPage.ClickSubmitButton();

            // Assert
            Assert.True(true);
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