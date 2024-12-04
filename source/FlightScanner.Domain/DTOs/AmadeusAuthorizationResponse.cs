using System.Text.Json.Serialization;

namespace FlightScanner.Domain.DTOs;

public class AmadeusAuthorizationResponse
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("username")]
    public string UserName { get; set; } = null!;

    [JsonPropertyName("application_name")]
    public string ApplicationName { get; set; } = null!;

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = null!;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = null!;

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; } = null!;

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = null!;
}
