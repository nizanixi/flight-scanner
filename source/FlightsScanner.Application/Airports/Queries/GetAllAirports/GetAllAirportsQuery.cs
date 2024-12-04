using FlightScanner.DTOs.Responses;
using MediatR;

namespace FlightsScanner.Application.Airports.Queries.GetAllAirports;

public class GetAllAirportsQuery : IRequest<AllAirportsResponseDto>
{
    public GetAllAirportsQuery()
    {
    }
}
