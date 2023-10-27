using Bogus;
using TechNews.UI.Tests.Configuration;
using TechNews.UI.Tests.Pages.Models;

namespace TechNews.UI.Tests.Fixtures;

[CollectionDefinition(nameof(TestsFixtureCollection))]
public class TestsFixtureCollection : ICollectionFixture<TestsFixture>
{
}

public class TestsFixture
{
    public readonly SeleniumHelper SeleniumHelper;

    public TestsFixture()
    {
        EnvironmentVariables.LoadVariables();

        SeleniumHelper = new SeleniumHelper(browser: Browser.Chrome, headless: true);
    }
    
    public UserModel GetValidUser()
    {
        var password = new Faker().Internet.Password(length: 8, memorable: false, prefix: "1aA@-");
        
        var user = new UserModel
        {
            Email = new Faker().Internet.Email(),
            UserName = new Faker().Internet.UserName(),
            Password = password,
            ConfirmPassword = password
        };

        return user;
    }
}
