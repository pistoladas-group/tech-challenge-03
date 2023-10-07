using System.ComponentModel.DataAnnotations;

namespace TechNews.Web.Models;

public class SignUpViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O campo E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} é inválido")]
    [MaxLength(256, ErrorMessage = "O campo E-mail deve ter um tamanho máximo de {1} caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo Nome de Usuário é obrigatório")]
    [MaxLength(256, ErrorMessage = "O campo Nome de Usuário deve ter um tamanho máximo de {1} caracteres")]
    [RegularExpression("^[a-zA-Z0-9-._@]*$", ErrorMessage = "O campo Nome de Usuário contém caracteres inválidos")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo Senha é obrigatório")]
    [MaxLength(128, ErrorMessage = "O campo Senha deve ter um tamanho máximo de {1} caracteres")]
    [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).+$", ErrorMessage = "O campo Senha deve conter pelo menos um digito, uma letra minúscula, uma maiúscula e um caracter especial")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo Confirmar Senha é obrigatório")]
    [MaxLength(128, ErrorMessage = "O campo Confirmar Senha deve ter um tamanho máximo de {1} caracteres")]
    [Compare("Password", ErrorMessage = "As senhas não conferem")]
    public string Repassword { get; set; } = string.Empty;
}