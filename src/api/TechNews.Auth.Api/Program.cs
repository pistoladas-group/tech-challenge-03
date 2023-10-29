using TechNews.Auth.Api.Configurations;
using TechNews.Common.Library.Middlewares;

namespace TechNews.Auth.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options => options.Filters.ConfigureFilters());

        builder.Services
                .AddEndpointsApiExplorer()
                .ConfigureSwagger()
                .AddEnvironmentVariables(builder.Environment)
                .AddLoggingConfiguration(builder.Host)
                .ConfigureIdentity()
                .ConfigureDatabase(builder.Environment)
                .ConfigureDependencyInjections()
                .AddHealthChecks();

        var app = builder.Build();

        app.UseSwaggerConfiguration();
        app.UseHsts();
        app.UseHttpsRedirection();
        app.UseMiddleware<ResponseHeaderMiddleware>();
        app.UseIdentityConfiguration();
        app.MapControllers();
        app.MapHealthChecks("/health");

        if (!builder.Environment.IsEnvironment("Testing"))
        {
            app.MigrateDatabase();
        }

        app.Run();
    }
}
