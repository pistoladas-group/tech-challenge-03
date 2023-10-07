using System.ComponentModel.DataAnnotations;

namespace TechNews.Web.Models;

public class SignInViewModel
{
    [Required(ErrorMessage = "O campo E-mail é obrigatório")]
    [MaxLength(256, ErrorMessage = "O campo E-mail deve ter um tamanho máximo de {1} caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo Senha é obrigatório")]
    [MaxLength(128, ErrorMessage = "O campo Senha deve ter um tamanho máximo de {1} caracteres")]
    public string Password { get; set; } = string.Empty;
}