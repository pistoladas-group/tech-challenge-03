using TechNews.Common.Library.Middlewares;
using TechNews.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
        .AddHttpClient()
        .AddAuthConfiguration()
        .AddEnvironmentVariables(builder.Environment)
        .ConfigureDependencyInjections()
        .AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = "InstrumentationKey=ae5e2077-ae37-42fb-860c-b409d1aa2280;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/";
        })
        .AddControllersWithViews(options => options.Filters.AddFilterConfiguration());

var app = builder.Build();

app.UseHsts();
app.UseHttpsRedirection();
app.UseMiddleware<ResponseHeaderMiddleware>();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthConfiguration();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
