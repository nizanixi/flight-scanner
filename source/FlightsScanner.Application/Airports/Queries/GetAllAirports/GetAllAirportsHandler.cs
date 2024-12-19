using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Exceptions;
using FlightsScanner.Application.Interfaces.Repositories;
using MediatR;

namespace FlightsScanner.Application.Airports.Queries.GetAllAirports;

public class GetAllAirportsHandler : IRequestHandler<GetAllAirportsQuery, IReadOnlyList<AirportEntity>>
{
    private readonly IAirportRepository _airportRepository;

    public GetAllAirportsHandler(IAirportRepository airportRepository)
    {
        _airportRepository = airportRepository;
    }

    public async Task<IReadOnlyList<AirportEntity>> Handle(GetAllAirportsQuery request, CancellationToken cancellationToken)
    {
        var airports = await _airportRepository.GetAllAirports(cancellationToken);

        if (airports == null || !airports.Any())
        {
            throw new InvalidResponseException($"There are not airports found!");
        }

        return airports;
    }
}
