using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechNews.Core.Api.Data;

namespace TechNews.Core.Api.IntegrationTests;

public class CoreApiFactory : WebApplicationFactory<Program>
{
    private string ConnectionString { get; set; }
    
    public CoreApiFactory(string connectionString)
    {
        ConnectionString = connectionString;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextOptionsDescriptor = services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (dbContextOptionsDescriptor is not null)
            {
                services.Remove(dbContextOptionsDescriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(ConnectionString);
            });
        });
    }
}
