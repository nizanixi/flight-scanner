namespace FlightScanner.Client.BlazorWA.Configurations;

public class BlazorWAConfiguration : IBlazorWAConfiguration
{
    private readonly IConfiguration _configuration;

    public BlazorWAConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;

        AirportsEndpointConfiguration = new AirportsEndpointConfiguration(
            configurationSection: configuration.GetSection("AirportsEndpointConfiguration"));
        FlightsEndpointConfiguration = new FlightsEndpointConfiguration(
            configurationSection: configuration.GetSection("FlightsEndpointConfiguration"));
    }

    public string BackendApiBaseUri => _configuration.GetValue<string>("FlightScannerBackendSettings:Audience")!;

    public AirportsEndpointConfiguration AirportsEndpointConfiguration { get; }

    public FlightsEndpointConfiguration FlightsEndpointConfiguration { get; }
}
