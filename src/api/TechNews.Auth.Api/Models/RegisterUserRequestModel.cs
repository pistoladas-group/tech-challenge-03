using System.ComponentModel.DataAnnotations;

namespace TechNews.Auth.Api.Models;

/// <summary>The User to be registered</summary>
public class RegisterUserRequestModel
{
    /// <summary>
    /// An optional user id in guid format. If not provided, it will be generated automatically
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// A valid user e-mail
    /// </summary>
    [Required(ErrorMessage = "The {0} field is mandatory")]
    [EmailAddress(ErrorMessage = "The {0} field is invalid")]
    [MaxLength(256, ErrorMessage = "The {0} field must have a maximum length of {1} characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// A valid user name. Allowed characters:
    /// abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@
    /// </summary>
    [Required(ErrorMessage = "The {0} field is mandatory")]
    [MaxLength(256, ErrorMessage = "The {0} field must have a maximum length of {1} characters")]
    [RegularExpression("^[a-zA-Z0-9-._@]*$", ErrorMessage = "The {0} field has invalid characters")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// A valid password. It must contain:
    /// - At least 8 characters long
    /// - One digit
    /// - One lowercase
    /// - One uppercase
    /// - One non alpha numeric
    /// </summary>
    [Required(ErrorMessage = "The {0} field is mandatory")]
    [MinLength(8, ErrorMessage = "The {0} field must have at least {1} characters")]
    [MaxLength(128, ErrorMessage = "The {0} field must have a maximum length of {1} characters")]
    [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).+$", ErrorMessage = "The {0} field must have at least one digit, one lowercase, one uppercase and a special character")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The password confirmation. It must be equals to the password
    /// </summary>
    [Required(ErrorMessage = "The {0} field is mandatory")]
    [MinLength(8, ErrorMessage = "The {0} field must have at least {1} characters")]
    [MaxLength(128, ErrorMessage = "The {0} field must have a maximum length of {1} characters")]
    [Compare("Password", ErrorMessage = "The passwords does not match")]
    public string Repassword { get; set; } = string.Empty;
}
