namespace TechNews.Common.Library.Models;

public class ErrorAppResponse
{
    public string? Error { get; set; }
    public string? ErrorCode { get; set; }
    public string? Description { get; set; }

    public ErrorAppResponse(string? error, string? errorCode, string? description)
    {
        Error = error;
        ErrorCode = errorCode;
        Description = description;
    }
}