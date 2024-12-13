using System.Text.Json.Serialization;

namespace FlightScanner.DTOs.Models.Amadeus;

public class AmadeusFlightSegmentDto
{
    /// <summary>
    /// Number of stops planned on the segment for technical or operation purpose i.e. refueling.
    /// </summary>
    [JsonPropertyName("numberOfStops")]
    public int NumberOfStops { get; set; }

    [JsonPropertyName("carrierCode")]
    public string AirlineCarrierCode { get; set; } = null!;

    [JsonPropertyName("departure")]
    public AmadeusFlightEndPointDto DepartureAirport { get; set; } = null!;

    [JsonPropertyName("arrival")]
    public AmadeusFlightEndPointDto ArrivalAirport { get; set; } = null!;
}
