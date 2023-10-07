using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TechNews.Web.Filters;

namespace TechNews.Web.Configurations;

public static class Filters
{
    public static void AddFilterConfiguration(this FilterCollection filterCollection)
    {
        filterCollection.Add<ModelStateFilter>();
        filterCollection.Add(new AutoValidateAntiforgeryTokenAttribute());
    }
}