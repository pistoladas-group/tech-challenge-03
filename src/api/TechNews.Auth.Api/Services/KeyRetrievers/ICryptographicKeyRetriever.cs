namespace TechNews.Auth.Api.Services.KeyRetrievers;

public interface ICryptographicKeyRetriever
{
    Task<ICryptographicKey?> GetExistingKeyAsync();
    Task StoreKeyAsync(ICryptographicKey key);
    Task<List<ICryptographicKey>> GetLastKeysAsync(int quantity);
}