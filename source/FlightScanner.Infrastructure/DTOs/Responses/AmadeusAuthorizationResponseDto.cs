using System.Text.Json.Serialization;

namespace FlightScanner.Infrastructure.DTOs.Responses;

public class AmadeusAuthorizationResponseDto
{
    /// <summary>
    /// The type of resource. The value will be amadeusOAuth2Token.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    /// <summary>
    /// Your username (email address).
    /// </summary>
    [JsonPropertyName("username")]
    public string UserName { get; set; } = null!;

    /// <summary>
    /// The name of your application.
    /// </summary>
    [JsonPropertyName("application_name")]
    public string ApplicationName { get; set; } = null!;

    /// <summary>
    /// The API Key for your application.
    /// </summary>
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = null!;

    /// <summary>
    /// The type of token issued by the authentication server. The value will be Bearer.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = null!;

    /// <summary>
    /// The token to authenticate your requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;

    /// <summary>
    /// The number of seconds until the token expires.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// The status of your request. Values can be approved or expired.
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = null!;
}
