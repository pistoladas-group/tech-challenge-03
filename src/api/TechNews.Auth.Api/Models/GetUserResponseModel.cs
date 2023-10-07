namespace TechNews.Auth.Api.Models;

/// <summary>
/// User information
/// </summary>
public class GetUserResponseModel
{
    /// <summary>
    /// A guid representing the user id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The user name
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The user email
    /// </summary>
    public string? Email { get; set; }
}
