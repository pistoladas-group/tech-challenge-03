using System.Text.Json.Serialization;
using TechNews.Common.Library.Middlewares;
using TechNews.Core.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.ConfigureFilters())
                .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services
    .AddEndpointsApiExplorer()
    .ConfigureSwagger()
    .AddEnvironmentVariables(builder.Environment)
    .AddLoggingConfiguration(builder.Host)
    .ConfigureDatabase()
    .ConfigureDependencyInjections()
    .AddAuthConfiguration()
    .AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = "InstrumentationKey=ae5e2077-ae37-42fb-860c-b409d1aa2280;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/";
    })
    .AddHealthChecks();


var app = builder.Build();

app.UseSwaggerConfiguration();
app.UseHttpsRedirection();
app.UseMiddleware<ResponseHeaderMiddleware>();
app.UseAuthConfiguration();
app.MapControllers();
app.MigrateDatabase();
app.MapHealthChecks("/health");
app.Run();
