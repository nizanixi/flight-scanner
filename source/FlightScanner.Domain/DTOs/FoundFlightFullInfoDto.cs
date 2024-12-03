using System.Text.Json.Serialization;

namespace FlightScanner.Domain.DTOs;

public class FoundFlightFullInfoDto
{
    [JsonPropertyName("departure")]
    public AirportDto DepartureAirport { get; set; } = null!;

    [JsonPropertyName("arrival")]
    public AirportDto ArrivalAirport { get; set; } = null!;
}
