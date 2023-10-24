using TechNews.Web.Configurations;
using TechNews.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
        .AddHttpClient()
        .AddAuthConfiguration()
        .AddEnvironmentVariables(builder.Environment)
        .ConfigureDependencyInjections()
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
