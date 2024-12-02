using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Exceptions;
using FlightScanner.Domain.Repositories;
using FlightsScanner.Application.Services.Contracts;
using MediatR;

namespace FlightsScanner.Application.Airports.Queries.GetAirport;

public class GetAirportHandler : IRequestHandler<GetAirportQuery, AirportEntity>
{
    private readonly IAirportRepository _airportRepository;
    private readonly IInMemoryCacheService _inMemoryCacheService;

    public GetAirportHandler(IAirportRepository airportRepository, IInMemoryCacheService inMemoryCacheService)
    {
        _airportRepository = airportRepository;
        _inMemoryCacheService = inMemoryCacheService;
    }

    public async Task<AirportEntity> Handle(GetAirportQuery request, CancellationToken cancellationToken)
    {
        var airport = await _inMemoryCacheService.TryGetCachedAirportItem(
            cacheKey: request.IataCode,
            getResultDelegate: () => _airportRepository.GetAirportWithIataCode(request.IataCode, cancellationToken));

        if (string.IsNullOrEmpty(airport.AirportName))
        {
            throw new InvalidResponseException($"Airport with code {airport.IataCode} doesn't have airport name!");
        }

        if (string.IsNullOrEmpty(airport.Location))
        {
            throw new InvalidResponseException($"Airport with code {airport.IataCode} doesn't have location!");
        }

        return airport;
    }
}
