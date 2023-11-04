using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechNews.Auth.Api.Data;

namespace TechNews.Auth.Api.IntegrationTests;

public class AuthApiFactory : WebApplicationFactory<Program>
{
    private string ConnectionString { get; set; }
    
    public AuthApiFactory(string connectionString)
    {
        ConnectionString = connectionString;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextOptionsDescriptor = services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<AuthDbContext>));

            if (dbContextOptionsDescriptor is not null)
            {
                services.Remove(dbContextOptionsDescriptor);
            }

            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(ConnectionString);
            });
        });
    }
}
