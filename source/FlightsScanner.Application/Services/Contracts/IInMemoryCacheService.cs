namespace FlightsScanner.Application.Services.Contracts;

public interface IInMemoryCacheService
{
    Task<T> TryGetCachedItem<T>(string cacheKey, Func<Task<T>> getResultDelegate);
}
