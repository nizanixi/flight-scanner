using System.Text.Json.Serialization;

namespace FlightScanner.Domain.DTOs;

public class FoundFlightsCollectionDto
{
    [JsonPropertyName("data")]
    public FoundFlightFullInfoDto[]? FlightInfoDtos { get; set; }
}
