using FlightScanner.DTOs.Models;

namespace FlightScanner.DTOs.Responses;

public class FoundFlightsResponseDto
{
    public FoundFlightsResponseDto(IReadOnlyList<FlightEntityDto> flightEntityDtos)
    {
        FlightEntityDtos = flightEntityDtos;
    }

    public IReadOnlyList<FlightEntityDto> FlightEntityDtos { get; }
}
