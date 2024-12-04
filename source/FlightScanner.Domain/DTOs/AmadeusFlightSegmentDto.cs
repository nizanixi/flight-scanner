using System.Text.Json.Serialization;

namespace FlightScanner.Domain.DTOs;

public class AmadeusFlightSegmentDto
{
    [JsonPropertyName("numberOfStops")]
    public int NumberOfStops { get; set; }

    [JsonPropertyName("carrierCode")]
    public string CarrierCode { get; set; } = null!;

    [JsonPropertyName("departure")]
    public AmadeusFlightEndPointDto DepartureAirport { get; set; } = null!;

    [JsonPropertyName("arrival")]
    public AmadeusFlightEndPointDto ArrivalAirport { get; set; } = null!;
}
