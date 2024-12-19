using System.Net.Http.Headers;
using FlightScanner.Common.Constants;
using FlightsScanner.Application.Interfaces.HttpClients;

namespace FlightScanner.Infrastructure.Authorization;
public class AuthorizationHttpClientHandler : DelegatingHandler
{
    private readonly IAmadeusAuthorizatoinHttpClient _amadeusAuthorizatoinHttpClient;

    public AuthorizationHttpClientHandler(IAmadeusAuthorizatoinHttpClient amadeusAuthorizatoinHttpClient)
    {
        _amadeusAuthorizatoinHttpClient = amadeusAuthorizatoinHttpClient;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _amadeusAuthorizatoinHttpClient.GetAuthorizationTokenAsync(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            scheme: HttpHeaderConstants.BEARER_AUTHORIZATION_SCHEME,
            parameter: token);

        return await base.SendAsync(request, cancellationToken);
    }
}
