using System.Text.Json.Serialization;

namespace FlightScanner.Domain.DTOs;

public class AmadeusFlightOfferPriceDto
{
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;

    [JsonPropertyName("total")]
    public decimal Total { get; set; }
}
