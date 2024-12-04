using System.Text.Json.Serialization;

namespace FlightScanner.DTOs.Models.Amadeus;

public class AmadeusFlightRouteDto
{
    [JsonPropertyName("segments")]
    public IEnumerable<AmadeusFlightSegmentDto> Segments { get; set; } = null!;
}
