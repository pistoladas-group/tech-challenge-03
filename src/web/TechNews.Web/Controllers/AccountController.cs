using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechNews.Web.Models;
using TechNews.Web.Configurations;
using TechNews.Common.Library.Models;

namespace TechNews.Web.Controllers;

[Authorize]
[Route("[controller]")]
public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpFactory;

    public AccountController(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    [AllowAnonymous]
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost("")]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpViewModel model)
    {
        var client = _httpFactory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var uri = $"{EnvironmentVariables.ApiAuthBaseUrl}/api/auth/user";

        var apiResponse = await client.PostAsync(uri, content);

        if (!apiResponse.IsSuccessStatusCode)
        {
            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResponse = await apiResponse.Content.ReadAsStringAsync();

                var response = JsonSerializer.Deserialize<AppResponse>(errorResponse, new JsonSerializerOptions(JsonSerializerDefaults.Web));

                response = TranslateRegisterErrors(response);

                return BadRequest(response);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        var responseString = await apiResponse.Content.ReadAsStringAsync();
        var appResponse = JsonSerializer.Deserialize<AppResponse>(responseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        if (appResponse?.Data is null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        var accessTokenResponse = JsonSerializer.Deserialize<AccessTokenResponse?>(appResponse.Data.ToString(), new JsonSerializerOptions(JsonSerializerDefaults.Web));

        var token = GetTokenFromString(accessTokenResponse?.AccessToken);

        if (token is null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        await AuthenticateUserByTokenAsync(token);

        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult Login()
    {
        return View("Login");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInViewModel model)
    {
        var client = _httpFactory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var uri = $"{EnvironmentVariables.ApiAuthBaseUrl}/api/auth/user/login";

        var apiResponse = await client.PostAsync(uri, content);

        if (!apiResponse.IsSuccessStatusCode)
        {
            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(new AppResponse() { Errors = new List<ErrorAppResponse>() { new ErrorAppResponse("invalid_request", "InvalidRequest", "Usuário ou senha inválidos") } });
            }

            if (apiResponse.StatusCode == HttpStatusCode.InternalServerError ||
                apiResponse.StatusCode == HttpStatusCode.Forbidden)
            {
                return StatusCode((int)apiResponse.StatusCode);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        var responseString = await apiResponse.Content.ReadAsStringAsync();
        var appResponse = JsonSerializer.Deserialize<AppResponse>(responseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        if (appResponse?.Data is null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        var accessTokenResponse = JsonSerializer.Deserialize<AccessTokenResponse?>(appResponse.Data.ToString(), new JsonSerializerOptions(JsonSerializerDefaults.Web));

        var token = GetTokenFromString(accessTokenResponse?.AccessToken);

        if (token is null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        await AuthenticateUserByTokenAsync(token);

        return Ok();
    }

    [HttpGet("logout")]
    public async Task<IActionResult> LogOutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Redirect("Login");
    }

    private static JwtSecurityToken? GetTokenFromString(string? jwtToken)
    {
        return new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken;
    }

    private async Task AuthenticateUserByTokenAsync(JwtSecurityToken token)
    {
        var claimsIdentity = GetClaimsByJwt(token);

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(EnvironmentVariables.AuthExpirationInMinutes),
            IsPersistent = true
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
    }

    private static ClaimsIdentity GetClaimsByJwt(JwtSecurityToken token)
    {
        var claims = new List<Claim>();
        claims.AddRange(token.Claims);
        claims.Add(new Claim("JWT", token.RawData));

        return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private static AppResponse? TranslateRegisterErrors(AppResponse? appResponse)
    {
        if (appResponse is null)
        {
            return null;
        }

        if (appResponse.Errors is null)
        {
            return appResponse;
        }

        for (int i = 0; i < appResponse.Errors.Count; i++)
        {
            if (appResponse.Errors[i].ErrorCode == "DuplicateUserName" ||
            appResponse.Errors[i].ErrorCode == "DuplicateEmail" ||
            appResponse.Errors[i].ErrorCode == "InvalidEmail" ||
            appResponse.Errors[i].ErrorCode == "InvalidUserName")
            {
                appResponse.Errors = new List<ErrorAppResponse>() { new ErrorAppResponse(appResponse.Errors[i].Error, appResponse.Errors[i].ErrorCode, "Usuário ou Email inválidos") };
                break;
            }
        }

        return appResponse;
    }
}
