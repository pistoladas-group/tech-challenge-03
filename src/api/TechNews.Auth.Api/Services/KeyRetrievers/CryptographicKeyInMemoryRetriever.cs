namespace TechNews.Auth.Api.Services.KeyRetrievers;

public class CryptographicKeyInMemoryRetriever : ICryptographicKeyRetriever
{
    private IList<ICryptographicKey> _keys { get; set; } = new List<ICryptographicKey>();

    public Task<ICryptographicKey?> GetExistingKeyAsync()
    {
        return Task.FromResult(_keys.OrderByDescending(k => k.CreationDate).FirstOrDefault());
    }

    public Task StoreKeyAsync(ICryptographicKey key)
    {
        _keys.Add(key);

        return Task.CompletedTask;
    }

    public Task<List<ICryptographicKey>> GetLastKeysAsync(int quantity)
    {
        return Task.FromResult(_keys.OrderByDescending(k => k.CreationDate).Take(quantity).ToList());
    }
}