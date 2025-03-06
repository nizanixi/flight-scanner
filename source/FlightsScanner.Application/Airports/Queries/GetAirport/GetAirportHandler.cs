using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Exceptions;
using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace FlightsScanner.Application.Airports.Queries.GetAirport;

public class GetAirportHandler : IRequestHandler<GetAirportQuery, AirportEntity>
{
    private readonly IAirportRepository _airportRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<GetAirportHandler> _logger;
    private readonly MemoryCacheEntryOptions _airportItemCacheEntryOptions;

    public GetAirportHandler(IAirportRepository airportRepository, IMemoryCache memoryCache, ILogger<GetAirportHandler> logger)
    {
        _airportRepository = airportRepository;
        _memoryCache = memoryCache;
        _logger = logger;

        _airportItemCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(CacheConstants.SLIDING_EXPIRATION_FOR_IATA_CODES_IN_SECONDS))
            .SetAbsoluteExpiration(TimeSpan.FromHours(CacheConstants.ABSOLUTE_EXPIRATION_FOR_IATA_CODES_IN_MINUTES))
            .SetPriority(CacheItemPriority.Normal)
            .SetSize(CacheConstants.IATA_CODE_CACHE_SIZE);
    }

    public async Task<AirportEntity> Handle(GetAirportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling airport request.");

        if (_memoryCache.TryGetValue(request.IataCode, out AirportEntity? cachedItem) && cachedItem != null)
        {
            return cachedItem;
        }

        var airport = await _airportRepository.GetAirportWithIataCode(request.IataCode, cancellationToken);

        if (string.IsNullOrEmpty(airport.AirportName))
        {
            throw new InvalidResponseException($"Airport with code {airport.IataCode} doesn't have airport name!");
        }

        if (string.IsNullOrEmpty(airport.Location))
        {
            throw new InvalidResponseException($"Airport with code {airport.IataCode} doesn't have location!");
        }

        _memoryCache.Set(
            key: request.IataCode,
            value: airport,
            options: _airportItemCacheEntryOptions);

        return airport;
    }
}
