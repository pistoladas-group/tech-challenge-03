using Microsoft.AspNetCore.Mvc.Filters;
using TechNews.Core.Api.Filters;

namespace TechNews.Core.Api.Configurations;

public static class Filters
{
    public static void ConfigureFilters(this FilterCollection filterCollection)
    {
        filterCollection.Add(new ModelStateFilter());
        filterCollection.Add(new ExceptionFilter());
    }
}