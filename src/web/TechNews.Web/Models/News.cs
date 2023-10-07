namespace TechNews.Web.Models;

public class News
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime PublishDate { get; set; }
    public string? ImageSource { get; set; }
    public Author Author { get; set; }
}