using System.Text.Json.Serialization;

namespace FlightScanner.Domain.DTOs;

public class AmadeusFlightRouteDto
{
    [JsonPropertyName("segments")]
    public IEnumerable<AmadeusFlightSegmentDto> Segments { get; set; } = null!;
}
