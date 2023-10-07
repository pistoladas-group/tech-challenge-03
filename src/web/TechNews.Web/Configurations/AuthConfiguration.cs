using Microsoft.AspNetCore.Authentication.Cookies;

namespace TechNews.Web.Configurations;

public static class AuthConfiguration
{
    public static IServiceCollection AddAuthConfiguration(this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;

                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(EnvironmentVariables.AuthExpirationInMinutes);
                options.SlidingExpiration = true;
                options.AccessDeniedPath = "/Forbidden/";
            });

        return services;
    }

    public static void UseAuthConfiguration(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}