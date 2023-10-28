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
        _registerPage = new RegisterPage(fixture.SeleniumHelper);
        _landingPage = new LandingPage(fixture.SeleniumHelper);
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
        var user = _fixture.GetValidUser();
        _registerPage.FillForm(user);

        // Assert
        Assert.True(_registerPage.CheckFormIsFilledUp());
    }

    [When(@"the register submit button is clicked")]
    public void WhenTheRegisterSubmitButtonIsClicked()
    {
        // Arrange & Act & Assert
        _registerPage.ClickSubmitButton();
    }

    [When(@"the register form is populated with an invalid e-mail")]
    public void WhenTheRegisterFormIsPopulatedWithAnInvalidEMail()
    {
        // Arrange & Act
        var user = _fixture.GetValidUser();
        user.Email = user.Email?.Remove(user.Email.IndexOf('@'), 1);
        _registerPage.FillForm(user);

        // Assert
        Assert.True(_registerPage.CheckFormIsFilledUp());
    }
    
    [When(@"the (.+) input loses focus")]
    public void WhenTheEMailInputLosesFocus(string formInputName)
    {
        // Arrange & Act & Assert
        _registerPage.ClickRegisterAccountTitle();
    }

    [Then(@"an invalid e-mail error message appears")]
    public void ThenAnInvalidEMailErrorMessageAppears()
    {
        // Arrange & Act
        var invalidMessageIsBeingShown = _registerPage.CheckInvalidEmailMessageIsBeingShown();
        
        // Assert
        Assert.True(invalidMessageIsBeingShown);
    }

    [When(@"the register form is populated with an invalid user name")]
    public void WhenTheRegisterFormIsPopulatedWithAnInvalidUserName()
    {
        // Arrange
        var user = _fixture.GetValidUser();
        user.UserName = user.UserName += '#';

        // Act
        _registerPage.FillForm(user);

        // Assert
        Assert.True(_registerPage.CheckFormIsFilledUp());
    }

    [Then(@"an invalid user name error message appears")]
    public void ThenAnInvalidUserNameErrorMessageAppears()
    {
        // Arrange & Act
        var invalidMessageIsBeingShown = _registerPage.CheckInvalidUserNameMessageIsBeingShown();
        
        // Assert
        Assert.True(invalidMessageIsBeingShown);
    }

    [When(@"the register form is populated with a weak password")]
    public void WhenTheRegisterFormIsPopulatedWithAWeekPassword()
    {
        // Arrange 
        var user = _fixture.GetValidUser();
        user.Password = user.Password.Replace("1aA@-", string.Empty);
        user.ConfirmPassword = user.Password;

        // Act
        _registerPage.FillForm(user);

        // Assert
        Assert.True(_registerPage.CheckFormIsFilledUp());
    }

    [Then(@"a week password error message appears")]
    public void ThenAWeekPasswordErrorMessageAppears()
    {
        // Arrange & Act 
        var invalidMessageIsBeingShown = _registerPage.CheckWeakPasswordMessageIsBeingShown();
        
        // Assert
        Assert.True(invalidMessageIsBeingShown);
    }
    
    [When(@"the register form is populated with passwords that doesn't match")]
    public void WhenTheRegisterFormIsPopulatedWithPasswordsThatDoesntMatch()
    {
        // Arrange 
        var user = _fixture.GetValidUser();
        user.ConfirmPassword += ".";

        // Act
        _registerPage.FillForm(user);

        // Assert
        Assert.True(_registerPage.CheckFormIsFilledUp());
    }

    [Then(@"a password doesnt match error message appears")]
    public void ThenAPasswordDoesntMatchErrorMessageAppears()
    {        
        // Arrange & Act 
        var invalidMessageIsBeingShown = _registerPage.CheckPasswordDoesntMatchErrorMessageIsBeingShown();
        
        //Assert
        Assert.True(invalidMessageIsBeingShown);
    }
}