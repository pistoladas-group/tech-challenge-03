using TechNews.Auth.Api.Data;

namespace TechNews.Auth.Api.Tests;

public class RoleTests : IClassFixture<TestsFixture>
{
    private TestsFixture _testsFixture { get; set; }

    public RoleTests(TestsFixture testsFixture)
    {
        _testsFixture = testsFixture;
    }

    [Fact(DisplayName = "ShouldInstantiateCorrectly_WhenConstructorCalled")]
    [Trait("Role Class", "")]
    public void Role_ShouldInstantiateCorrectly_WhenConstructorCalled()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var user = new Role(guid);

        // Assert
        Assert.Equal(guid, user.Id);
        Assert.Equal(createdAt.Date, user.CreatedAt.Date);
        Assert.Equal(createdAt.Hour, user.CreatedAt.Hour);
        Assert.Equal(createdAt.Minute, user.CreatedAt.Minute);
        Assert.False(user.IsDeleted);
    }
}