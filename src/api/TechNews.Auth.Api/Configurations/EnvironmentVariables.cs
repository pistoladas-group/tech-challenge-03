using System.Text;
using dotenv.net;

namespace TechNews.Auth.Api.Configurations;

public static class EnvironmentVariables
{
    public static string? DatabaseConnectionString { get; private set; }
    public static string? DiscordWebhookId { get; private set; }
    public static string? DiscordWebhookToken { get; private set; }
    public static int TokenExpirationInMinutes { get; private set; }
    public static int KeyRotatorExecutionInMinutes { get; private set; }
    public static int KeyCreationSizeInBits { get; private set; }
    public static int KeyExpirationInDays { get; private set; }
    public static string? CryptographicAlgorithm { get; private set; }
    public static string AzureKeyVaultUrl { get; private set; } = string.Empty;

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
        DatabaseConnectionString = Environment.GetEnvironmentVariable("TECHNEWS_AUTH_API_DATABASE_CONNECTION_STRING");
        DiscordWebhookId = Environment.GetEnvironmentVariable("TECHNEWS_AUTH_API_DISCORD_WEBHOOK_ID");
        DiscordWebhookToken = Environment.GetEnvironmentVariable("TECHNEWS_AUTH_API_DISCORD_WEBHOOK_TOKEN");
        CryptographicAlgorithm = Environment.GetEnvironmentVariable("CRYPTOGRAPHIC_ALGORITHM");
        AzureKeyVaultUrl = Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_URL") ?? string.Empty;

        int.TryParse(Environment.GetEnvironmentVariable("TOKEN_EXPIRATION_IN_MINUTES"), out var parsedExpiration);
        TokenExpirationInMinutes = parsedExpiration;

        if (parsedExpiration == 0)
        {
            TokenExpirationInMinutes = 10;
        }

        int.TryParse(Environment.GetEnvironmentVariable("KEY_ROTATOR_EXECUTION_IN_MINUTES"), out var parsedExecution);
        KeyRotatorExecutionInMinutes = parsedExecution;

        if (parsedExecution == 0)
        {
            KeyRotatorExecutionInMinutes = 5;
        }

        int.TryParse(Environment.GetEnvironmentVariable("KEY_CREATION_SIZE_IN_BITS"), out var parsedSize);
        KeyCreationSizeInBits = parsedSize;

        if (parsedSize == 0)
        {
            KeyCreationSizeInBits = 2048;
        }

        int.TryParse(Environment.GetEnvironmentVariable("KEY_EXPIRATION_IN_DAYS"), out var parsedDays);
        KeyExpirationInDays = parsedDays;

        if (parsedDays == 0)
        {
            KeyExpirationInDays = 30;
        }
    }
}