using TechNews.Auth.Api.Configurations;
using TechNews.Auth.Api.Middlewares;

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
        .AddHealthChecks();

var app = builder.Build();

app.UseSwaggerConfiguration();
app.UseHsts();
app.UseHttpsRedirection();
app.UseMiddleware<ResponseHeaderMiddleware>();
app.UseIdentityConfiguration();
app.MapControllers();
app.MapHealthChecks("/health");
app.MigrateDatabase();
app.Run();
