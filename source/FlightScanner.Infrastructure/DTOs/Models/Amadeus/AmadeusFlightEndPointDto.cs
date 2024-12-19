using System.Text.Json.Serialization;

namespace FlightScanner.Infrastructure.DTOs.Models.Amadeus;

public class AmadeusFlightEndPointDto
{
    [JsonPropertyName("iataCode")]
    public string IataCode { get; set; } = null!;

    [JsonPropertyName("terminal")]
    public string Terminal { get; set; } = null!;

    [JsonPropertyName("at")]
    public DateTime DateTime { get; set; }
}
