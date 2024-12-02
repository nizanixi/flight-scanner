namespace FlightsScanner.Application.Services.Contracts;

public interface IInMemoryCacheService
{
    Task<T> TryGetCachedAirportItem<T>(string cacheKey, Func<Task<T>> getResultDelegate);
}
