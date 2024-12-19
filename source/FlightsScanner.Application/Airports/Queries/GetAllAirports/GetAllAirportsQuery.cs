using FlightScanner.Domain.Entities;
using MediatR;

namespace FlightsScanner.Application.Airports.Queries.GetAllAirports;

public class GetAllAirportsQuery : IRequest<IReadOnlyList<AirportEntity>>
{
    public GetAllAirportsQuery()
    {
    }
}
