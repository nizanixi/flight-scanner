using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using FlightScanner.Common.Constants;
using FlightScanner.DTOs.Responses;
using FlightsScanner.Application.Configurations;
using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace FlightsScanner.Application.Services.Implementations;

public class AmadeusAuthorizatoinHandlerService : IAmadeusAuthorizatoinHandlerService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AmadeusEndpointConfiguration _amadeusEndpointConfiguration;
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _amadeusAuthorizationCacheEntryOptions;

    public AmadeusAuthorizatoinHandlerService(IHttpClientFactory httpClientFactory, AmadeusEndpointConfiguration amadeusEndpointConfiguration, IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _amadeusEndpointConfiguration = amadeusEndpointConfiguration;
        _memoryCache = memoryCache;

        _amadeusAuthorizationCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(CacheConstants.SLIDING_EXPIRATION_FOR_AMADEUS_AUTHORIZATION))
            .SetAbsoluteExpiration(TimeSpan.FromHours(CacheConstants.ABSOLUTE_EXPIRATION_FOR_AMADEUS_AUTHORIZATION))
            .SetPriority(CacheItemPriority.High)
            .SetSize(CacheConstants.AMADEUS_AUTHORIZATION_CACHE_SIZE);
    }

    public async Task<string> GetAuthorizationTokenAsync(bool searchCachedToken)
    {
        if (searchCachedToken
            && _memoryCache.TryGetValue(_amadeusEndpointConfiguration.AmadeusFlightSearchApiKey, out string? cachedItem)
            && !string.IsNullOrEmpty(cachedItem))
        {
            return cachedItem;
        }

        var accessToken = await ExecuteHttpPostForGettingAccessToken();

        _memoryCache.Set(
            key: _amadeusEndpointConfiguration.AmadeusFlightSearchApiKey,
            value: accessToken,
            options: _amadeusAuthorizationCacheEntryOptions);

        return accessToken;
    }

    private async Task<string> ExecuteHttpPostForGettingAccessToken()
    {
        var requestUri = $"{_amadeusEndpointConfiguration.BaseUri}/{_amadeusEndpointConfiguration.AuthenticationEndpoint}";

        var grantUrl = $"{_amadeusEndpointConfiguration.GrantTypeHeader}={_amadeusEndpointConfiguration.ClientCredentialsGrantType}" +
                    $"&{_amadeusEndpointConfiguration.ClientIdHeader}={_amadeusEndpointConfiguration.AmadeusFlightSearchApiKey}" +
                    $"&{_amadeusEndpointConfiguration.ClientSecretHeader}={_amadeusEndpointConfiguration.AmadeusFlightSearchApiSecret}";

        var content = new StringContent(
            content: grantUrl,
            encoding: Encoding.UTF8,
            mediaType: MediaTypeNames.Application.FormUrlEncoded);

        var httpClient = _httpClientFactory.CreateClient(HttpClientConstants.DEFAULT_HTTP_CLIENT_NAME);
        httpClient.DefaultRequestHeaders.Add(HttpHeaderConstants.ACCEPT_TYPE, MediaTypeNames.Application.Json);

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
