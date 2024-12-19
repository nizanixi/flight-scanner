using System.Text.Json.Serialization;

namespace FlightScanner.Infrastructure.DTOs.Models.Aviation;

public class FoundFlightFullInfoDto
{
    [JsonPropertyName("departure")]
    public AviationAirportDto DepartureAirport { get; set; } = null!;

    [JsonPropertyName("arrival")]
    public AviationAirportDto ArrivalAirport { get; set; } = null!;
}
