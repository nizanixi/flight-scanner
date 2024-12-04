using FlightScanner.DTOs.Models.Amadeus;
using System.Text.Json.Serialization;

namespace FlightScanner.DTOs.Responses;

public class AmadeusFlightsResponseDto
{
    [JsonPropertyName("data")]
    public IEnumerable<AmadeusFlightOffersDto> FlightOffers { get; set; } = null!;

    [JsonPropertyName("dictionaries")]
    public FlightVocalbularyDto FlightVocalbulary { get; set; } = null!;
}
