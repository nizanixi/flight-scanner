using FlightScanner.DTOs.Responses;
using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Services.Contracts;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace FlightsScanner.Application.Flights.Queries.GetFlights;

internal class GetFlightsHandler : IRequestHandler<GetFlightsQuery, FoundFlightsResponseDto>
{
    private readonly IFlightSearchService _flightSearchService;
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _flightItemCacheEntryOptions;

    public GetFlightsHandler(IFlightSearchService flightSearchService, IMemoryCache memoryCache)
    {
        _flightSearchService = flightSearchService;
        _memoryCache = memoryCache;

        _flightItemCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(CacheConstants.SLIDING_EXPIRATION_FOR_FLIGHTS_IN_SECONDS))
            .SetAbsoluteExpiration(TimeSpan.FromHours(CacheConstants.ABSOLUTE_EXPIRATION_FOR_FLIGHTS_CODES_IN_MINUTES))
            .SetPriority(CacheItemPriority.Normal)
            .SetSize(CacheConstants.FLIGHT_ITEM_CACHE_SIZE);
    }

    public async Task<FoundFlightsResponseDto> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
    {
        var flightHashCodeKey = CreateUniqueCacheKeyForGetFlightsRequest(request);
        if (_memoryCache.TryGetValue(flightHashCodeKey, out FoundFlightsResponseDto? cachedItem) && cachedItem != null)
        {
            return cachedItem;
        }

        var flights = await _flightSearchService.GetFlights(
            request.DepartureAirportIataCode,
            request.DepartureTime,
            request.DestinationAirportIataCode,
            request.ReturnTripTime,
            request.NumberOfPassengers,
            request.Currency);

        _memoryCache.Set(
            key: flightHashCodeKey,
            value: flights,
            options: _flightItemCacheEntryOptions);

        return flights;
    }

    private static int CreateUniqueCacheKeyForGetFlightsRequest(GetFlightsQuery request)
    {
        return HashCode.Combine(
            request.DepartureAirportIataCode,
            request.DepartureTime,
            request.DestinationAirportIataCode,
            request.ReturnTripTime,
            request.NumberOfPassengers,
            request.Currency);
    }
}
