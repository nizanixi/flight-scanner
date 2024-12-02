using FlightScanner.Domain.Entities;
using MediatR;

namespace FlightsScanner.Application.Airports.Queries.GetAirport;

public class GetAirportQuery : IRequest<AirportEntity>
{
    public GetAirportQuery(string iataCode)
    {
        IataCode = iataCode;
    }

    public string IataCode { get; }
}
