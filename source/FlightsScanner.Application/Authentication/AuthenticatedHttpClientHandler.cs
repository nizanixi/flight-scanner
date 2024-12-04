using System.Net;
using System.Net.Http.Headers;
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
        var token = await _amadeusAuthorizatoinHandlerService.GetAuthorizationTokenAsync(searchCachedToken: true);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            scheme: "Bearer",
            parameter: token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        token = await _amadeusAuthorizatoinHandlerService.GetAuthorizationTokenAsync(searchCachedToken: false);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            scheme: "Bearer",
            parameter: token);

        return await base.SendAsync(request, cancellationToken);
    }
}
