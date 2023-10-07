using System.ComponentModel.DataAnnotations;

namespace TechNews.Auth.Api.Models;

/// <summary>
/// The user to login
/// </summary>
public class LoginRequestModel
{
    /// <summary>
    /// The user email
    /// </summary>
    [Required(ErrorMessage = "The {0} field is mandatory")]
    [MaxLength(256, ErrorMessage = "The {0} field must have a maximum length of {1} characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user password
    /// </summary>
    [Required(ErrorMessage = "The {0} field is mandatory")]
    [MaxLength(128, ErrorMessage = "The {0} field must have a maximum length of {1} characters")]
    public string Password { get; set; } = string.Empty;
}
