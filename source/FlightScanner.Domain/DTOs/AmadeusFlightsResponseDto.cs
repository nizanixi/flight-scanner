using System.Text.Json.Serialization;

namespace FlightScanner.Domain.DTOs;

public class AmadeusFlightsResponseDto
{
    [JsonPropertyName("data")]
    public IEnumerable<AmadeusFlightOffersDto> FlightOffers { get; set; } = null!;

    [JsonPropertyName("dictionaries")]
    public FlightVocalbularyDto FlightVocalbulary { get; set; } = null!;
}
