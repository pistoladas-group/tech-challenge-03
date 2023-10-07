using System.Text;
using dotenv.net;

namespace TechNews.Web.Configurations;

public static class EnvironmentVariables
{
    public static string? ApiAuthBaseUrl { get; private set; }
    public static string? ApiCoreBaseUrl { get; private set; }
    public static int AuthExpirationInMinutes { get; private set; }

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
        ApiAuthBaseUrl = Environment.GetEnvironmentVariable("TECHNEWS_APP_API_AUTH_URL");
        ApiCoreBaseUrl = Environment.GetEnvironmentVariable("TECHNEWS_APP_API_CORE_URL");

        int.TryParse(Environment.GetEnvironmentVariable("AUTH_EXPIRATION_IN_MINUTES"), out var parsedExpiration);
        AuthExpirationInMinutes = parsedExpiration;

        if (parsedExpiration == 0)
        {
            AuthExpirationInMinutes = 20;
        }
    }
}