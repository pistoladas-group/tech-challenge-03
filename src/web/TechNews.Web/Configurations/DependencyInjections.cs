namespace TechNews.Web.Configurations;

public static class DependencyInjections
{
    public static IServiceCollection ConfigureDependencyInjections(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        return services;
    }
}