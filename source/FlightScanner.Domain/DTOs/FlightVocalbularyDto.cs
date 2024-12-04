using System.Text.Json.Serialization;

namespace FlightScanner.Domain.DTOs;

public class FlightVocalbularyDto
{
    [JsonPropertyName("carriers")]
    public IDictionary<string, string> Carriers { get; set; } = null!;

    [JsonPropertyName("aircraft")]
    public IDictionary<string, string> Airplanes { get; set; } = null!;
}
