namespace TechNews.Common.Library.Models;

public class ApiResponse
{
    public bool Succeeded { get; private set; }
    public object? Data { get; private set; }
    public List<ErrorResponse>? Errors { get; private set; }

    public ApiResponse()
    {
        Succeeded = true;
    }

    public ApiResponse(object? data)
    {
        Succeeded = true;
        Data = data;
    }

    public ApiResponse(ErrorResponse error)
    {
        Succeeded = false;
        Errors ??= new List<ErrorResponse>();
        Errors.Add(error);
    }

    public ApiResponse(List<ErrorResponse> errors)
    {
        Succeeded = false;
        Errors = errors;
    }
}