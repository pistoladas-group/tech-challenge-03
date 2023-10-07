using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace TechNews.Common.Library.Services;

public class JwksRetriever : IConfigurationRetriever<OpenIdConnectConfiguration>
{
    public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(string address, IDocumentRetriever retriever, CancellationToken cancel)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentNullException(nameof(address));
        }

        if (retriever is null)
        {
            throw new ArgumentNullException(nameof(retriever));
        }

        IdentityModelEventSource.ShowPII = true;

        var doc = await retriever.GetDocumentAsync(address, cancel);
        var jwks = new JsonWebKeySet(doc);

        var openIdConnectConfiguration = new OpenIdConnectConfiguration()
        {
            JsonWebKeySet = jwks,
            JwksUri = address,
        };

        foreach (var securityKey in jwks.GetSigningKeys())
        {
            openIdConnectConfiguration.SigningKeys.Add(securityKey);
        }

        return openIdConnectConfiguration;
    }
}
