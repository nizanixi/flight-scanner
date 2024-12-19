using System.Text.Json.Serialization;
using FlightScanner.Infrastructure.DTOs.Models.Aviation;

namespace FlightScanner.Infrastructure.DTOs.Responses;

public class AviationFlightsResponseDto
{
    [JsonPropertyName("data")]
    public FoundFlightFullInfoDto[]? FlightInfoDtos { get; set; }
}
