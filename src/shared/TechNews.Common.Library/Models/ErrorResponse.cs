using System.Text.Json.Serialization;

namespace TechNews.Common.Library.Models;

public class ErrorResponse
{
    public string? Error { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? Description { get; private set; }

    public ErrorResponse(string? error, string? errorCode, string? description)
    {
        Error = error;
        ErrorCode = errorCode;
        Description = description;
    }
}