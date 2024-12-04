using FlightScanner.DTOs.Models;

namespace FlightScanner.DTOs.Responses;

public class AllAirportsResponseDto
{
    public AllAirportsResponseDto(IReadOnlyList<AirportDto> airports)
    {
        Airports = airports;
    }

    public IReadOnlyList<AirportDto> Airports { get; }
}
