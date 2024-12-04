using FlightScanner.DTOs.Responses;
using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;

namespace FlightsScanner.Application.Services.Implementations;

public class AmadeusAuthorizatoinHandlerService : IAmadeusAuthorizatoinHandlerService
{
    private const string AMADUES_API_KEY_CONFIGURATION_KEY = "AmadeusFlightSearchApiKey";
    private const string AMADEUS_API_SECRET_CONFIGURATION_KEY = "AmadeusFlightSearchApiSecret";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly string _amadeusFlightSearchApiKey;
    private readonly string _amadeusFlightSearchApiSecret;
    private readonly MemoryCacheEntryOptions _amadeusAuthorizationCacheEntryOptions;

    public AmadeusAuthorizatoinHandlerService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;

        _amadeusFlightSearchApiKey = configuration[AMADUES_API_KEY_CONFIGURATION_KEY]
            ?? throw new ArgumentNullException("Aviation stack API key not found!");
        _amadeusFlightSearchApiSecret = configuration[AMADEUS_API_SECRET_CONFIGURATION_KEY]
            ?? throw new ArgumentNullException("Aviation stack API key not found!");

        _amadeusAuthorizationCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(CacheConstants.SLIDING_EXPIRATION_FOR_AMADEUS_AUTHORIZATION))
            .SetAbsoluteExpiration(TimeSpan.FromHours(CacheConstants.ABSOLUTE_EXPIRATION_FOR_AMADEUS_AUTHORIZATION))
            .SetPriority(CacheItemPriority.High)
            .SetSize(CacheConstants.AMADEUS_AUTHORIZATION_CACHE_SIZE);
    }

    public async Task<string> GetAuthorizationTokenAsync(bool searchCachedToken)
    {
        if (searchCachedToken
            && _memoryCache.TryGetValue(CacheConstants.AMADEUS_FLIGHT_SEARCH_API_KEY, out string? cachedItem)
            && !string.IsNullOrEmpty(cachedItem))
        {
            return cachedItem;
        }

        var accessToken = await ExecuteHttpPostForGettingAccessToken();

        _memoryCache.Set(
            key: CacheConstants.AMADEUS_FLIGHT_SEARCH_API_KEY,
            value: accessToken,
            options: _amadeusAuthorizationCacheEntryOptions);

        return accessToken;
    }

    private async Task<string> ExecuteHttpPostForGettingAccessToken()
    {
        var requestUri = $"{UriConstants.AMADEUS_BASE_REQUEST_URI}/{UriConstants.AMADEUS_AUTH_ENDPOINT}";

        var content = new StringContent(
            content: $"grant_type=client_credentials&client_id={_amadeusFlightSearchApiKey}&client_secret={_amadeusFlightSearchApiSecret}",
            encoding: Encoding.UTF8,
            mediaType: MediaTypeNames.Application.FormUrlEncoded);

        var httpClient = _httpClientFactory.CreateClient(HttpClientConstants.DEFAULT_HTTP_CLIENT_NAME);
        httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        var httpResponse = await httpClient.PostAsync(requestUri, content);
        httpResponse.EnsureSuccessStatusCode();
        var amadeusAuthorizationResponse = await httpResponse.Content.ReadFromJsonAsync<AmadeusAuthorizationResponseDto>();

        if (amadeusAuthorizationResponse == null)
        {
            throw new Exception();
        }

        return amadeusAuthorizationResponse.AccessToken;
    }
}
