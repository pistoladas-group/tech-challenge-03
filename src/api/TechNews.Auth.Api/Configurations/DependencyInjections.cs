using TechNews.Auth.Api.Services;
using TechNews.Auth.Api.Services.Cryptography;
using TechNews.Auth.Api.Services.KeyRetrievers;

namespace TechNews.Auth.Api.Configurations;

public static class DependencyInjections
{
    public static IServiceCollection ConfigureDependencyInjections(this IServiceCollection services)
    {
        // Scopeds
        switch (EnvironmentVariables.CryptographicAlgorithm)
        {
            case "ECC":
                services.AddScoped<ICryptographicKeyFactory, EcdsaCryptographicKeyFactory>();
                services.AddScoped<ICryptographicKey, EcdsaCryptographicKey>();
                break;
            case "RSA":
            default:
                services.AddScoped<ICryptographicKeyFactory, RsaCryptographicKeyFactory>();
                services.AddScoped<ICryptographicKey, RsaCryptographicKey>();
                break;
        }

        // Singletons
        services.AddSingleton<ICryptographicKeyRetriever, CryptographicKeyAzureVaultRetriever>();

        // Background Services
        services.AddHostedService<KeyRotatorBackgroundService>();

        return services;
    }
}