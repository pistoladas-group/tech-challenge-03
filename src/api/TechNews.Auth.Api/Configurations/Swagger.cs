using System.Reflection;
using Microsoft.OpenApi.Models;

namespace TechNews.Auth.Api.Configurations;

public static class Swagger
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Tech News - Auth API",
                Description = "The authentication API",
                TermsOfService = new Uri("https://github.com/pistoladas-group/tech-challenge-02"),
                Contact = new OpenApiContact
                {
                    Name = "Github Issues - Tech News project",
                    Url = new Uri("https://github.com/pistoladas-group/tech-challenge-02/issues"),
                    Email = "pistoladas-group@gmail.com"
                }
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        return services;
    }

    public static void UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}