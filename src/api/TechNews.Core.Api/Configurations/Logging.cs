using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;

namespace TechNews.Core.Api.Configurations;

public static class Logging
{
    public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, ConfigureHostBuilder host)
    {
        var webhookId = Convert.ToUInt64(EnvironmentVariables.DiscordWebhookId);
        var webhookToken = EnvironmentVariables.DiscordWebhookToken;
        
        host.UseSerilog((_, logConfiguration) => logConfiguration
            .WriteTo.Console(LogEventLevel.Verbose)
            .WriteTo.Discord(webhookId: webhookId, webhookToken: webhookToken, restrictedToMinimumLevel: LogEventLevel.Warning)
            .MinimumLevel.Verbose()
        );
        
        return services;
    }
}