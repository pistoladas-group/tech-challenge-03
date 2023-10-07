using TechNews.Common.Library.Models;

namespace TechNews.Core.Api.Data.Models;

public class News : Entity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime PublishDate { get; set; }
    public string? ImageSource { get; set; }
    public Guid AuthorId { get; set; }
    public Author Author { get; set; }

    //EF
    protected News()
    {
    }

    public News(string title, string description, DateTime publishDate, Author author, string? imageSource)
    {
        Title = title;
        Description = description;
        PublishDate = publishDate;
        Author = author;
        ImageSource = imageSource;
    }
}