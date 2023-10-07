using System.Text.Json.Serialization;

namespace TechNews.Auth.Api.Models;

public class JwksResponseModel
{
    [JsonPropertyName("keys")]
    public List<JsonWebKeyModel> Keys { get; set; } = new List<JsonWebKeyModel>();
}