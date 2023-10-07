using System.Net;
using Microsoft.AspNetCore.Mvc;
using TechNews.Auth.Api.Models;
using TechNews.Auth.Api.Services.KeyRetrievers;
using TechNews.Common.Library.Models;

namespace TechNews.Auth.Api.Controllers;

[Route("jwks")]
public class JwksController : ControllerBase
{
    private readonly ICryptographicKeyRetriever _cryptographicKeyRetriever;

    public JwksController(ICryptographicKeyRetriever rsaKeyRetriever)
    {
        _cryptographicKeyRetriever = rsaKeyRetriever;
    }

    /// <summary>
    /// Gets the Json Web Key Sets
    /// </summary>
    /// <response code="200">Returns the Json Web Key Sets if any</response>
    /// <response code="500">There was an internal problem</response>
    [HttpGet()]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetJsonWebKeySetsAsync()
    {
        var keys = await _cryptographicKeyRetriever.GetLastKeysAsync(2);

        var result = new JwksResponseModel();

        if (keys is null)
        {
            return Ok(result);
        }

        foreach (var key in keys)
        {
            var jwk = key.GetJsonWebKey();

            if (jwk is not null)
            {
                result.Keys.Add(jwk);
            }
        }

        return Ok(result);
    }
}