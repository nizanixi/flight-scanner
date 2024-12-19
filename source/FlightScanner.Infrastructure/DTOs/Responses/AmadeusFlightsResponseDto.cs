using System.Text.Json.Serialization;
using FlightScanner.Infrastructure.DTOs.Models.Amadeus;

namespace FlightScanner.Infrastructure.DTOs.Responses;

public class AmadeusFlightsResponseDto
{
    [JsonPropertyName("data")]
    public IEnumerable<AmadeusFlightOffersDto> FlightOffers { get; set; } = null!;

    [JsonPropertyName("dictionaries")]
    public FlightVocalbularyDto FlightVocalbulary { get; set; } = null!;
}
