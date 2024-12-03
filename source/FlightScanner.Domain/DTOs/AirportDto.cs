using System.Text.Json.Serialization;

namespace FlightScanner.Domain.DTOs;

public class AirportDto
{
    [JsonPropertyName("airport")]
    public string Airport { get; set; } = null!;

    [JsonPropertyName("iata")]
    public string IataCode { get; set; } = null!;

    [JsonPropertyName("scheduled")]
    public string Sheduled { get; set; } = null!;
}
