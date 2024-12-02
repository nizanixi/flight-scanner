using FlightScanner.Domain.Entities;
using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace FlightsScanner.Application.Infrastructure;

public class InMemoryCacheService : IInMemoryCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _airportItemCacheEntryOptions;
    private readonly MemoryCacheEntryOptions _flightItemCacheEntryOptions;

    public InMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;

        _airportItemCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(CacheConstants.SLIDING_EXPIRATION_FOR_IATA_CODES_IN_SECONDS))
            .SetAbsoluteExpiration(TimeSpan.FromHours(CacheConstants.ABSOLUTE_EXPIRATION_FOR_IATA_CODES_IN_MINUTES))
            .SetPriority(CacheItemPriority.Normal)
            .SetSize(CacheConstants.IATA_CODE_CACHE_SIZE);

        _flightItemCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(CacheConstants.SLIDING_EXPIRATION_FOR_FLIGHTS_IN_SECONDS))
            .SetAbsoluteExpiration(TimeSpan.FromHours(CacheConstants.ABSOLUTE_EXPIRATION_FOR_FLIGHTS_CODES_IN_MINUTES))
            .SetPriority(CacheItemPriority.Normal)
            .SetSize(CacheConstants.FLIGHT_ITEM_CACHE_SIZE);
    }

    public async Task<T> TryGetCachedAirportItem<T>(string cacheKey, Func<Task<T>> getResultDelegate)
    {
        MemoryCacheEntryOptions cacheExpiration;
        if (typeof(T) == typeof(AirportEntity))
        {
            cacheExpiration = _airportItemCacheEntryOptions;
        }
        else if (typeof(T) != typeof(AirportEntity))
        {
            cacheExpiration = _flightItemCacheEntryOptions;
        }
        else
        {
            throw new NotImplementedException($"Caching not implemented for key: {cacheKey}");
        }

        if (_memoryCache.TryGetValue(cacheKey, out T? cachedItem) && cachedItem != null)
        {
            return cachedItem;
        }

        var result = await getResultDelegate();

        _memoryCache.Set(
            key: cacheKey,
            value: result,
            options: cacheExpiration);

        return result;
    }
}
