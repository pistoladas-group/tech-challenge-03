using Microsoft.AspNetCore.Mvc.Filters;
using TechNews.Auth.Api.Filters;

namespace TechNews.Auth.Api.Configurations;

public static class Filters
{
    public static void ConfigureFilters(this FilterCollection filterCollection)
    {
        filterCollection.Add(new ModelStateFilter());
        filterCollection.Add(new ExceptionFilter());
    }
}