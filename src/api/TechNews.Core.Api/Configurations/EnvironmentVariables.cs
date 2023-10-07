using System.Text;
using dotenv.net;

namespace TechNews.Core.Api.Configurations;

public static class EnvironmentVariables
{
    public static string? DatabaseConnectionString { get; private set; }
    public static string? DiscordWebhookId { get; private set; }
    public static string? DiscordWebhookToken { get; private set; }
    public static string? AuthJwksUrl { get; private set; }

    public static IServiceCollection AddEnvironmentVariables(this IServiceCollection services, IWebHostEnvironment environment)
    {
        try
        {
            DotEnv.Fluent()
                .WithExceptions()
                .WithEnvFiles()
                .WithTrimValues()
                .WithEncoding(Encoding.UTF8)
                .WithOverwriteExistingVars()
                .WithProbeForEnv(probeLevelsToSearch: 6)
                .Load();
        }
        catch (Exception)
        {
            if (environment.IsEnvironment("Local"))
            {
                throw new ApplicationException("Environment File (.env) not found. The application needs a .env file to run locally.\nPlease check the section Environment Variables of the README.");
            }

            // Ignored if other environments because it is using runtime environment variables
        }

        LoadVariables();

        return services;
    }

    private static void LoadVariables()
    {
        DatabaseConnectionString = Environment.GetEnvironmentVariable("TECHNEWS_CORE_API_DATABASE_CONNECTION_STRING");
        DiscordWebhookId = Environment.GetEnvironmentVariable("TECHNEWS_CORE_API_DISCORD_WEBHOOK_ID");
        DiscordWebhookToken = Environment.GetEnvironmentVariable("TECHNEWS_CORE_API_DISCORD_WEBHOOK_TOKEN");
        AuthJwksUrl = Environment.GetEnvironmentVariable("TECHNEWS_CORE_API_AUTH_JWKS_URL");
    }
}