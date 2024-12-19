namespace FlightsScanner.Application.Interfaces.HttpClients;

public interface IAmadeusAuthorizatoinHttpClient
{
    Task<string> GetAuthorizationTokenAsync(CancellationToken cancellationToken);
}
