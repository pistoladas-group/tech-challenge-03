using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using TechNews.Auth.Api.Configurations;
using TechNews.Auth.Api.Services.Cryptography;

namespace TechNews.Auth.Api.Services.KeyRetrievers;

public class CryptographicKeyAzureVaultRetriever : ICryptographicKeyRetriever
{
    private const string SecretName = "CryptographicPrivateKey";
    private readonly ICryptographicKeyFactory _cryptographicKeyFactory;

    public CryptographicKeyAzureVaultRetriever(ICryptographicKeyFactory cryptographicKeyFactory)
    {
        _cryptographicKeyFactory = cryptographicKeyFactory;
    }

    // TODO: Ter alguma forma de cache distribuído (ex: redis) da chave privada
    // para evitar ficar buscando toda hora no vault, sendo que a chave demora para mudar
    public async Task<ICryptographicKey?> GetExistingKeyAsync()
    {
        var client = new SecretClient(new Uri(EnvironmentVariables.AzureKeyVaultUrl), new DefaultAzureCredential());

        try
        {
            var retrievedSecret = await client.GetSecretAsync(SecretName);
            var privateKeyBytes = retrievedSecret.Value.Value;

            return _cryptographicKeyFactory.CreateFromPrivateKey(privateKeyBytes);
        }
        catch (RequestFailedException ex)
        {
            if (ex.Status == (int)System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            throw;
        }
    }

    public async Task StoreKeyAsync(ICryptographicKey key)
    {
        var base64Bytes = key.GetBase64PrivateKeyBytes();

        var client = new SecretClient(new Uri(EnvironmentVariables.AzureKeyVaultUrl), new DefaultAzureCredential());
        var secret = new KeyVaultSecret(SecretName, base64Bytes);

        await client.SetSecretAsync(secret);
    }

    // TODO: Ter alguma forma de cache distribuído (ex: redis) da chave privada
    // para evitar ficar buscando toda hora no vault, sendo que a chave demora para mudar
    public async Task<List<ICryptographicKey>> GetLastKeysAsync(int quantity)
    {
        var result = new List<ICryptographicKey>();

        var client = new SecretClient(new Uri(EnvironmentVariables.AzureKeyVaultUrl), new DefaultAzureCredential());

        try
        {
            await foreach (var property in client.GetPropertiesOfSecretVersionsAsync(SecretName).OrderByDescending(p => p.CreatedOn).Take(quantity))
            {
                var retrievedSecret = await client.GetSecretAsync(SecretName, property.Version);
                var privateKeyBytes = retrievedSecret.Value.Value;

                result.Add(_cryptographicKeyFactory.CreateFromPrivateKey(privateKeyBytes));
            }

            return result;
        }
        catch (RequestFailedException ex)
        {
            if (ex.Status == (int)System.Net.HttpStatusCode.NotFound)
            {
                return result;
            }

            throw;
        }
    }
}