using System.Text;
using dotenv.net;

namespace TechNews.UI.Tests.Configuration;

public static class EnvironmentVariables
{
    public static string TechNewsWebUri { get; private set; } = string.Empty;
    public static int MaxSecondsWaitingForPage { get; private set; } = 10;
    public static string ScreenshotsFolderPath { get; private set; } = string.Empty;


    public static void LoadVariables()
    {
        LoadEnvFile();

        TechNewsWebUri = Environment.GetEnvironmentVariable("TECH_NEWS_WEB_URI") ?? string.Empty;
        ScreenshotsFolderPath = Environment.GetEnvironmentVariable("SCREENSHOTS_FOLDER_PATH") ?? string.Empty;

        int.TryParse(Environment.GetEnvironmentVariable("MAX_SECONDS_WAITING_FOR_PAGE"), out var parsedTimeout);

        if (parsedTimeout > 0)
        {
            MaxSecondsWaitingForPage = parsedTimeout;
        }
    }

    private static void LoadEnvFile()
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
            // Ignored if .env not found because it is using runtime environment variables
        }
    }
}
