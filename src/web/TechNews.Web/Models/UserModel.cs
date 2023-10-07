namespace TechNews.Web.Models;

public class UserModel
{
    public bool IsAuthenticated { get; internal set; }
    public string UserName { get; set; }
}