using Serilog;
using TechNews.Auth.Api.Configurations;
using TechNews.Auth.Api.Services.Cryptography;
using TechNews.Auth.Api.Services.KeyRetrievers;

namespace TechNews.Auth.Api.Services;

public class KeyRotatorBackgroundService : IHostedService, IDisposable
{
    private const string ServiceName = "Key Rotator";
    private Timer? _timer;
    private bool _isProcessing;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public KeyRotatorBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(async _ => await ExecuteProcessAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(EnvironmentVariables.KeyRotatorExecutionInMinutes));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("Background service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    private async Task ExecuteProcessAsync()
    {
        if (_isProcessing)
        {
            Log.Debug("{service}: last execution still running. Aborting.", ServiceName);
            return;
        }

        _isProcessing = true;
        Log.Debug("{service}: started.", ServiceName);

        using var scope = _serviceScopeFactory.CreateScope();
        var keyRetrieverService = scope.ServiceProvider.GetRequiredService<ICryptographicKeyRetriever>();

        var existingKey = await keyRetrieverService.GetExistingKeyAsync();
        var isKeyValid = existingKey?.IsValid() ?? false;

        if (!isKeyValid)
        {
            var factory = scope.ServiceProvider.GetRequiredService<ICryptographicKeyFactory>();
            existingKey = factory.CreateKey();
            await keyRetrieverService.StoreKeyAsync(existingKey);
        }

        if (existingKey is null)
        {
            Log.Error("{service}: An error occurred when trying to get the RSA key.", ServiceName);
            throw new ApplicationException("An error occurred when trying to get the RSA key");
        }

        _isProcessing = false;
        Log.Debug("{service}: finished.", ServiceName);
    }
}
