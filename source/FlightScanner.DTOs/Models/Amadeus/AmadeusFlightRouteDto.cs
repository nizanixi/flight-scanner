using System.Text.Json.Serialization;

namespace FlightScanner.DTOs.Models.Amadeus;

public class AmadeusFlightRouteDto
{
    [JsonPropertyName("segments")]
    public IReadOnlyList<AmadeusFlightSegmentDto> RouteSegments { get; set; } = null!;

    /// <summary>
    /// This is returned in format ISO 8601.
    /// </summary>
    [JsonPropertyName("duration")]
    public string Duration { get; set; } = null!;
}
