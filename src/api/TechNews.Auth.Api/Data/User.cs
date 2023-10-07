using Microsoft.AspNetCore.Identity;

namespace TechNews.Auth.Api.Data;

public sealed class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    public User(Guid id, string email, string userName)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
        Email = email;
        UserName = userName;
    }
}
