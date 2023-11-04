using System.Text.Json;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace TechNews.Core.Api.IntegrationTests;

public class TestsFixture : IDisposable, IAsyncLifetime
{
    private CoreApiFactory _coreApiFactory = null!;
    public HttpClient CoreHttpClient = null!;
    
    private MsSqlContainer _sqlServerContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPortBinding(hostPort: 1434, containerPort: 1433)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
        .WithPassword("Pass@123")
        .WithEnvironment(name: "ACCEPT_EULA", value: "Y")
        .WithEnvironment(name: "MSSQL_PID", value: "developer")
        .Build();
    
    public T? Deserialize<T>(string data)
    {
        return string.IsNullOrEmpty(data) 
            ? default 
            : JsonSerializer.Deserialize<T>(data, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }
    
    public void Dispose()
    {
        _coreApiFactory.Dispose();
        CoreHttpClient.Dispose();
    }

    public async Task InitializeAsync()
    {
        await _sqlServerContainer.StartAsync();

        _coreApiFactory = new CoreApiFactory(_sqlServerContainer.GetConnectionString());
        CoreHttpClient = _coreApiFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services => 
                {
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                });
            })  
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
    }

    public async Task DisposeAsync()
    {
        await _sqlServerContainer.StopAsync();
    }
}
