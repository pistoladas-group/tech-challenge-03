using TechNews.Auth.Api.Configurations;
using TechNews.Common.Library.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.ConfigureFilters());

builder.Services
    .AddEndpointsApiExplorer()
    .ConfigureSwagger()
    .AddEnvironmentVariables(builder.Environment)
    .AddLoggingConfiguration(builder.Host)
    .ConfigureIdentity()
    .ConfigureDatabase()
    .ConfigureDependencyInjections()
    .AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = "InstrumentationKey=ae5e2077-ae37-42fb-860c-b409d1aa2280;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/";
    })
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