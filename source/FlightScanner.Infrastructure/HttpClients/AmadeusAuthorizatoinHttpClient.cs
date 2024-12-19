using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using FlightScanner.Common.Constants;
using FlightScanner.DTOs.Responses;
using FlightScanner.Infrastructure.Configurations;
using FlightScanner.Infrastructure.Constants;
using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Interfaces.HttpClients;
using Microsoft.Extensions.Caching.Memory;

namespace FlightScanner.Infrastructure.HttpClients;

public class AmadeusAuthorizatoinHttpClient : IAmadeusAuthorizatoinHttpClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AmadeusEndpointConfiguration _amadeusEndpointConfiguration;
    private readonly IMemoryCache _memoryCache;

    public AmadeusAuthorizatoinHttpClient(IHttpClientFactory httpClientFactory, AmadeusEndpointConfiguration amadeusEndpointConfiguration, IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _amadeusEndpointConfiguration = amadeusEndpointConfiguration;
        _memoryCache = memoryCache;
    }

    public async Task<string> GetAuthorizationTokenAsync(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue(_amadeusEndpointConfiguration.AmadeusFlightSearchApiKey, out string? cachedItem)
            && !string.IsNullOrEmpty(cachedItem))
        {
            return cachedItem;
        }

        var amadeusAuthorizationDto = await ExecuteHttpPostForGettingAccessToken(cancellationToken);

        var cacheSettings = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(amadeusAuthorizationDto.ExpiresIn))
            .SetAbsoluteExpiration(TimeSpan.FromHours(amadeusAuthorizationDto.ExpiresIn))
            .SetPriority(CacheItemPriority.High)
            .SetSize(CacheConstants.AMADEUS_AUTHORIZATION_CACHE_SIZE);

        _memoryCache.Set(
            key: _amadeusEndpointConfiguration.AmadeusFlightSearchApiKey,
            value: amadeusAuthorizationDto.AccessToken,
            options: cacheSettings);

        return amadeusAuthorizationDto.AccessToken;
    }

    private async Task<AmadeusAuthorizationResponseDto> ExecuteHttpPostForGettingAccessToken(CancellationToken cancellationToken)
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

        var httpResponse = await httpClient.PostAsync(requestUri, content, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        var amadeusAuthorizationResponse = await httpResponse.Content.ReadFromJsonAsync<AmadeusAuthorizationResponseDto>(cancellationToken);

        if (amadeusAuthorizationResponse == null)
        {
            throw new Exception();
        }

        return amadeusAuthorizationResponse;
    }
}
