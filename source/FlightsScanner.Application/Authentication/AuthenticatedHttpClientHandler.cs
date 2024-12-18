using System.Net;
using System.Net.Http.Headers;
using FlightScanner.Common.Constants;
using FlightsScanner.Application.Services.Contracts;

namespace FlightsScanner.Application.Authentication;
public class AuthenticatedHttpClientHandler : DelegatingHandler
{
    private readonly IAmadeusAuthorizatoinHandlerService _amadeusAuthorizatoinHandlerService;

    public AuthenticatedHttpClientHandler(IAmadeusAuthorizatoinHandlerService amadeusAuthorizatoinHandlerService)
    {
        _amadeusAuthorizatoinHandlerService = amadeusAuthorizatoinHandlerService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _amadeusAuthorizatoinHandlerService.GetAuthorizationTokenAsync(searchCachedToken: true, cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            scheme: HttpHeaderConstants.BEARER_AUTHORIZATION_SCHEME,
            parameter: token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        // Cached token is out of date, fetch new one and save it to cache
        token = await _amadeusAuthorizatoinHandlerService.GetAuthorizationTokenAsync(searchCachedToken: false, cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            scheme: HttpHeaderConstants.BEARER_AUTHORIZATION_SCHEME,
            parameter: token);

        return await base.SendAsync(request, cancellationToken);
    }
}
