using Bogus;
using TechNews.Auth.Api.Data;

namespace TechNews.Auth.Api.Tests;

public class UserTests : IClassFixture<TestsFixture>
{
    private TestsFixture _testsFixture { get; set; }

    public UserTests(TestsFixture testsFixture)
    {
        _testsFixture = testsFixture;
    }

    [Fact(DisplayName = "ShouldInstantiateCorrectly_WhenConstructorCalled")]
    [Trait("User Class", "")]
    public void User_ShouldInstantiateCorrectly_WhenConstructorCalled()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var email = new Faker().Internet.Email();
        var userName = new Faker().Internet.UserName();
        var createdAt = DateTime.UtcNow;

        // Act
        var user = new User(guid, email, userName);

        // Assert
        Assert.Equal(guid, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(userName, user.UserName);
        Assert.Equal(createdAt.Date, user.CreatedAt.Date);
        Assert.Equal(createdAt.Hour, user.CreatedAt.Hour);
        Assert.Equal(createdAt.Minute, user.CreatedAt.Minute);
        Assert.False(user.IsDeleted);
    }
}