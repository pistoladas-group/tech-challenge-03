using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using TechNews.Common.Library.Services;

namespace TechNews.Core.Api.Configurations;

public static class AuthConfiguration
{
    public static IServiceCollection AddAuthConfiguration(this IServiceCollection services)
    {
        var retrievalUrl = EnvironmentVariables.AuthJwksUrl;

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
            AddJwtBearer(options =>
            {
                if (retrievalUrl is null)
                {
                    throw new ApplicationException(nameof(retrievalUrl));
                }

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                var httpClient = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler())
                {
                    Timeout = options.BackchannelTimeout,
                    MaxResponseContentBufferSize = 1024 * 1024 * 10 // 10 MB 
                };

                options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    retrievalUrl,
                    new JwksRetriever(),
                    new HttpDocumentRetriever(httpClient) { RequireHttps = options.RequireHttpsMetadata }
                );

                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.ValidateLifetime = true;
                options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;

                var jwksUri = new Uri(retrievalUrl);
                options.TokenValidationParameters.ValidIssuer = $"{jwksUri.Scheme}://{jwksUri.Authority}";
            });

        return services;
    }

    public static void UseAuthConfiguration(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}