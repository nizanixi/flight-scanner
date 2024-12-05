using System.Text.Json.Serialization;
using FlightScanner.DTOs.Models.Aviation;

namespace FlightScanner.DTOs.Responses;

public class AviationFlightsResponseDto
{
    [JsonPropertyName("data")]
    public FoundFlightFullInfoDto[]? FlightInfoDtos { get; set; }
}
