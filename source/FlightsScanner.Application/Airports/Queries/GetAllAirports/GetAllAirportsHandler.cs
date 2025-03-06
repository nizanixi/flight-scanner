using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Exceptions;
using FlightsScanner.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightsScanner.Application.Airports.Queries.GetAllAirports;

public class GetAllAirportsHandler : IRequestHandler<GetAllAirportsQuery, IReadOnlyList<AirportEntity>>
{
    private readonly IAirportRepository _airportRepository;
    private readonly ILogger<GetAllAirportsHandler> _logger;

    public GetAllAirportsHandler(IAirportRepository airportRepository, ILogger<GetAllAirportsHandler> logger)
    {
        _airportRepository = airportRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<AirportEntity>> Handle(GetAllAirportsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting airports from repository");

        var airports = await _airportRepository.GetAllAirports(cancellationToken);

        if (airports == null || !airports.Any())
        {
            throw new InvalidResponseException($"There are not airports found!");
        }

        return airports;
    }
}
