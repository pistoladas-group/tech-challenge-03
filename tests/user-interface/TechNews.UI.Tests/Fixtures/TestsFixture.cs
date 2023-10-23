using TechNews.UI.Tests.Configuration;

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

        SeleniumHelper = new SeleniumHelper(browser: Browser.Chrome, headless: false);
    }
}
