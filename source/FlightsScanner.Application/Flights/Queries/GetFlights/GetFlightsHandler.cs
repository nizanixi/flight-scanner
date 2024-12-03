using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Services;
using FlightsScanner.Application.Services.Contracts;
using MediatR;

namespace FlightsScanner.Application.Flights.Queries.GetFlights;

internal class GetFlightsHandler : IRequestHandler<GetFlightsQuery, IReadOnlyList<FlightEntity>>
{
    private readonly IFlightSearchService _flightSearchService;
    private readonly IInMemoryCacheService _inMemoryCacheService;

    public GetFlightsHandler(IFlightSearchService flightSearchService, IInMemoryCacheService inMemoryCacheService)
    {
        _flightSearchService = flightSearchService;
        _inMemoryCacheService = inMemoryCacheService;
    }

    public async Task<IReadOnlyList<FlightEntity>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
    {
        var getFlightsDelegate = () => _flightSearchService.GetFlights(
            request.DepartureAirportIataCode,
            request.DepartureTime,
            request.DestinationAirportIataCode,
            request.ReturnTripTime,
            request.NumberOfPassengers,
            request.Currency);

        var flightHashCodeKey = request.GetHashCode().ToString();

        var flights = await _inMemoryCacheService.TryGetCachedItem(
            cacheKey: flightHashCodeKey,
            getResultDelegate: getFlightsDelegate);

        return flights;
    }
}
