using Microsoft.EntityFrameworkCore;
using TechNews.Auth.Api.Data;

namespace TechNews.Auth.Api.Configurations;

public static class Database
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services)
    {
        var connectionString = EnvironmentVariables.DatabaseConnectionString;

        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseSqlServer(connectionString, assembly =>
                assembly.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName)
                        .EnableRetryOnFailure(maxRetryCount: 2, maxRetryDelay: TimeSpan.FromSeconds(2), errorNumbersToAdd: null)
                        .MigrationsHistoryTable("_AppliedMigrations"));
        });

        return services;
    }

    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        dbContext.Database.Migrate();
    }
}