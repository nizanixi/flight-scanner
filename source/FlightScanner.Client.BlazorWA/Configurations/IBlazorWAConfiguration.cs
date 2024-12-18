namespace FlightScanner.Client.BlazorWA.Configurations;

public interface IBlazorWAConfiguration
{
    string BackendApiBaseUri { get; }

    AirportsEndpointConfiguration AirportsEndpointConfiguration { get; }

    FlightsEndpointConfiguration FlightsEndpointConfiguration { get; }
}
