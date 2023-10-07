namespace TechNews.Common.Library.Models;

public class AppResponse
{
    public bool Succeeded { get; set; }
    public object? Data { get; set; }
    public List<ErrorAppResponse>? Errors { get; set; }
}