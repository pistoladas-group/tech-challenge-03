using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechNews.Common.Library.Models;
using TechNews.Web.Configurations;
using TechNews.Web.Models;

namespace TechNews.Web.Controllers;

[Authorize]
public class NewsController : Controller
{
    private readonly IHttpClientFactory _httpFactory;

    public NewsController(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpFactory.CreateClient();
        var uri = $"{EnvironmentVariables.ApiCoreBaseUrl}/api/news";
        var token = HttpContext.User.Claims.First(x => x.Type == "JWT").Value;

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var apiResponse = await client.SendAsync(requestMessage);

        if (apiResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            return RedirectToAction("Login", "Account");
        }

        var serializedResponse = await apiResponse.Content.ReadAsStringAsync();
        var deserialzedResponse = JsonSerializer.Deserialize<AppResponse>(serializedResponse, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        if (!apiResponse.IsSuccessStatusCode)
        {
            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(deserialzedResponse);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, deserialzedResponse);
        }

        var model = new List<News>();

        if (deserialzedResponse?.Data is not null)
        {
            model = JsonSerializer.Deserialize<List<News>>(deserialzedResponse.Data.ToString(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }

        model?.ForEach(x =>
        {
            x.Description = x.Description.Length >= 300 ? $"{x.Description[..300]}..." : $"{x.Description}...";
            x.ImageSource = $"{x.ImageSource}/200";
        });

        return View(model);
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        var client = _httpFactory.CreateClient();
        var uri = $"{EnvironmentVariables.ApiCoreBaseUrl}/api/news/{id}";
        var token = HttpContext.User.Claims.First(x => x.Type == "JWT").Value;

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var apiResponse = await client.SendAsync(requestMessage);

        if (apiResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            return RedirectToAction("Login", "Account");
        }

        var serializedResponse = await apiResponse.Content.ReadAsStringAsync();
        var deserialzedResponse = JsonSerializer.Deserialize<AppResponse>(serializedResponse, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        if (!apiResponse.IsSuccessStatusCode)
        {
            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(deserialzedResponse);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, deserialzedResponse);
        }

        var model = new News();

        if (deserialzedResponse?.Data is not null)
        {
            model = JsonSerializer.Deserialize<News>(deserialzedResponse.Data.ToString(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }

        model.ImageSource = $"{model.ImageSource}/800/400";

        return View(model);
    }
}
