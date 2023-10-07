using System.Net;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TechNews.Auth.Api.Data;
using TechNews.Auth.Api.Models;
using TechNews.Auth.Api.Configurations;
using TechNews.Common.Library.Models;
using TechNews.Auth.Api.Services.KeyRetrievers;

namespace TechNews.Auth.Api.Controllers;

[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ICryptographicKeyRetriever _cryptographicKeyRetriever;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, ICryptographicKeyRetriever cryptographicKeyRetriever)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _cryptographicKeyRetriever = cryptographicKeyRetriever;
    }

    /// <summary>
    /// Creates a new User
    /// </summary>
    /// <param name="user">The user to be registered</param>
    /// <response code="201">Returns the created resource endpoint in response header</response>
    /// <response code="400">There is a problem with the request</response>
    /// <response code="500">There was an internal problem</response>
    [HttpPost("user")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserRequestModel user)
    {
        var id = user.Id ?? Guid.NewGuid();

        var existingUser = await _userManager.FindByIdAsync(id.ToString());

        if (existingUser is not null)
        {
            return BadRequest(new ApiResponse(error: new ErrorResponse("invalid_request", "UserAlreadyExists", "User already exists")));
        }

        var createUserResult = await _userManager.CreateAsync(new User(id, user.Email, user.UserName), user.Password);

        if (!createUserResult.Succeeded)
        {
            return BadRequest(new ApiResponse(errors: createUserResult.Errors.ToList().ConvertAll(x => new ErrorResponse("invalid_request", x.Code, x.Description))));
        }

        var registeredUserResult = await _userManager.FindByEmailAsync(user.Email);

        if (registeredUserResult is null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse(error: new ErrorResponse("server_error", "InternalError", "There was an unexpected error with the application. Please contact support!")));
        }

        var claims = await GetUserClaims(registeredUserResult);

        var token = await GetTokenAsync(claims, registeredUserResult);

        if (token is null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse(error: new ErrorResponse("server_error", "InternalError", "There was an unexpected error with the application. Please contact support!")));
        }

        return CreatedAtAction(nameof(GetUser), new { userId = id }, new ApiResponse(data: token));
    }

    /// <summary>
    /// Get the user details
    /// </summary>
    /// <param name="userId">The user id to be searched</param>
    /// <response code="200">Returns the resource data</response>
    /// <response code="400">There is a problem with the request</response>
    /// <response code="404">There is no resource with the given id</response>
    /// <response code="500">There was an internal problem</response>
    [HttpGet("user/{userId:guid}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUser([FromRoute] Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest(new ApiResponse(error: new ErrorResponse("invalid_request", "InvalidUser", "The userId is not valid")));
        }

        var getUserResult = await _userManager.FindByIdAsync(userId.ToString());

        if (getUserResult is null)
        {
            return NotFound(new ApiResponse(error: new ErrorResponse("invalid_request", "UserNotFound", "The user was not found")));
        }

        var responseModel = new GetUserResponseModel
        {
            Id = getUserResult.Id,
            UserName = getUserResult.UserName,
            Email = getUserResult.Email,
        };

        return Ok(new ApiResponse(data: responseModel));
    }

    /// <summary>
    /// Login an user
    /// </summary>
    /// <param name="user">The user to be logged in</param>
    /// <response code="201">Returns the user login data</response>
    /// <response code="400">There is a problem with the request</response>
    /// <response code="404">The informed user was not found</response>
    /// <response code="500">There was an internal problem</response>
    [HttpPost("user/login")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestModel user)
    {
        var registeredUserResult = await _userManager.FindByEmailAsync(user.Email);

        if (registeredUserResult is null || registeredUserResult?.UserName is null)
        {
            return BadRequest(new ApiResponse(error: new ErrorResponse("invalid_request", "InvalidRequest", "User or password are invalid")));
        }

        var signInResult = await _signInManager.PasswordSignInAsync(registeredUserResult.UserName, user.Password, false, true);

        if (signInResult.IsLockedOut)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(error: new ErrorResponse("unauthorized_client", "LockedUser", "User temporary blocked for invalid attempts")));
        }

        if (!signInResult.Succeeded)
        {
            return BadRequest(new ApiResponse(error: new ErrorResponse("invalid_request", "InvalidRequest", "User or password are invalid")));
        }

        var claims = await GetUserClaims(registeredUserResult);

        var token = await GetTokenAsync(claims, registeredUserResult);

        if (token is null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse(error: new ErrorResponse("server_error", "InternalError", "There was an unexpected error with the application. Please contact support!")));
        }

        return Ok(new ApiResponse(data: token));
    }

    private async Task<AccessTokenResponse?> GetTokenAsync(ClaimsIdentity claims, User user)
    {
        var tokenClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()),
            new(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)
        };

        claims.AddClaims(tokenClaims);

        var key = await _cryptographicKeyRetriever.GetExistingKeyAsync();

        if (key is null)
        {
            return null;
        }

        var tokenType = "at+jwt";

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}",
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(EnvironmentVariables.TokenExpirationInMinutes),
            TokenType = tokenType,
            SigningCredentials = key.GetSigningCredentials()
        });

        var jwt = tokenHandler.WriteToken(token);

        return new AccessTokenResponse()
        {
            AccessToken = jwt,
            TokenType = tokenType,
            ExpiresInSeconds = EnvironmentVariables.TokenExpirationInMinutes * 60
        };
    }

    private static long ToUnixEpochDate(DateTime date)
    {
        return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }

    private async Task<ClaimsIdentity> GetUserClaims(User registeredUserResult)
    {
        var claims = new ClaimsIdentity();
        var userClaims = await _userManager.GetClaimsAsync(registeredUserResult);
        var userRoles = await _userManager.GetRolesAsync(registeredUserResult);

        claims.AddClaims(userClaims);
        foreach (var userRole in userRoles)
        {
            claims.AddClaim(new Claim("role", userRole));
        }

        return claims;
    }
}