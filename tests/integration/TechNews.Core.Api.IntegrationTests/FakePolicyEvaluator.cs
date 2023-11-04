using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace TechNews.Core.Api.IntegrationTests;

public class FakePolicyEvaluator : IPolicyEvaluator
{
    public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        var principal = new ClaimsPrincipal();

        principal.AddIdentity(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, "Integration Test"),
            new Claim(ClaimTypes.NameIdentifier, "Integration Test")
        }, "TestScheme"));

        return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, new AuthenticationProperties(), "TestScheme")));
    }

    public virtual async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object resource)
    {
        return await Task.FromResult(PolicyAuthorizationResult.Success());
    }
}