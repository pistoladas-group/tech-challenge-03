using TechNews.Common.Library.Models;

namespace TechNews.Core.Api.Data.Models;

public class Author : Entity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string? ImageSource { get; set; }
    public List<News> News { get; set; } = new List<News>();

    //EF
    protected Author()
    {
    }

    public Author(string name, string email, string? imageSource)
    {
        Name = name;
        Email = email;
        ImageSource = imageSource;
    }
}