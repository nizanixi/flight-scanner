using FlightScanner.Domain.Repositories;
using FlightScanner.DTOs.Models;
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
    private readonly IAirportRepository _airportRepository;
    private readonly MemoryCacheEntryOptions _flightItemCacheEntryOptions;
    private readonly MemoryCacheEntryOptions _iataCodesWithAirportsCacheEntryOptions;

    public GetFlightsHandler(IFlightSearchService flightSearchService, IMemoryCache memoryCache, IAirportRepository airportRepository)
    {
        _flightSearchService = flightSearchService;
        _memoryCache = memoryCache;
        _airportRepository = airportRepository;

        _flightItemCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(CacheConstants.SLIDING_EXPIRATION_FOR_FLIGHTS_IN_SECONDS))
            .SetAbsoluteExpiration(TimeSpan.FromHours(CacheConstants.ABSOLUTE_EXPIRATION_FOR_FLIGHTS_CODES_IN_MINUTES))
            .SetPriority(CacheItemPriority.Normal)
            .SetSize(CacheConstants.FLIGHT_ITEM_CACHE_SIZE);

        _iataCodesWithAirportsCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(CacheConstants.SLIDING_EXPIRATION_FOR_IATA_CODES_IN_SECONDS))
            .SetAbsoluteExpiration(TimeSpan.FromHours(CacheConstants.ABSOLUTE_EXPIRATION_FOR_IATA_CODES_IN_MINUTES))
            .SetPriority(CacheItemPriority.Normal)
            .SetSize(CacheConstants.IATA_CODE_CACHE_SIZE);
    }

    public async Task<FoundFlightsResponseDto> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
    {
        var flightHashCodeKey = CreateUniqueCacheKeyForGetFlightsRequest(request);
        if (_memoryCache.TryGetValue(flightHashCodeKey, out FoundFlightsResponseDto? cachedItem)
            && cachedItem != null)
        {
            return cachedItem;
        }

        var flights = await _flightSearchService.GetFlights(
            request.DepartureAirportIataCode,
            request.DepartureTime,
            request.DestinationAirportIataCode,
            request.ReturnTripTime,
            request.NumberOfPassengers,
            request.Currency,
            cancellationToken);

        await MapAirportIataCodesToAirportLocations(flights.FlightEntityDtos, cancellationToken);

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

    private async Task MapAirportIataCodesToAirportLocations(IEnumerable<FlightEntityDto> flightEntityDtos, CancellationToken cancellationToken)
    {
        var allIataAirportCodes = flightEntityDtos
            .SelectMany(f => new[] { f.DepartureAirportIataCode, f.ArrivalAirportIataCode })
            .Distinct();

        var iataCodesWithLocations = new Dictionary<string, string>();
        foreach (var iataCode in allIataAirportCodes)
        {
            if (_memoryCache.TryGetValue(iataCode, out string? cachedLocation)
                && !string.IsNullOrEmpty(cachedLocation))
            {
                iataCodesWithLocations[iataCode] = cachedLocation;

                continue;
            }

            var airtportDto = await _airportRepository.GetAirportWithIataCode(iataCode, cancellationToken);

            iataCodesWithLocations[iataCode] = airtportDto.Location;

            _memoryCache.Set(
                key: iataCode,
                value: airtportDto.Location,
                options: _iataCodesWithAirportsCacheEntryOptions);
        }

        foreach (var flightEntity in flightEntityDtos)
        {
            flightEntity.ArrivalAirportLocation = iataCodesWithLocations[flightEntity.ArrivalAirportIataCode];
            flightEntity.DepartureAirportLocation = iataCodesWithLocations[flightEntity.DepartureAirportIataCode];
        }
    }
}
