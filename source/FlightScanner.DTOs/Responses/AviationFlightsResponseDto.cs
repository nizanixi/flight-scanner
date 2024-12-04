using FlightScanner.DTOs.Models.Aviation;
using System.Text.Json.Serialization;

namespace FlightScanner.DTOs.Responses;

public class AviationFlightsResponseDto
{
    [JsonPropertyName("data")]
    public FoundFlightFullInfoDto[]? FlightInfoDtos { get; set; }
}
