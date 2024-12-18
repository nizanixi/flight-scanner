namespace FlightScanner.Client.BlazorWA.Configurations;

public class AirportsEndpointConfiguration
{
    private readonly IConfigurationSection _configurationSection;

    public AirportsEndpointConfiguration(IConfigurationSection configurationSection)
    {
        _configurationSection = configurationSection;
    }

    public string GetAllAirportsEndpoint => _configurationSection.GetValue<string>("GetAllAirportsEndpoint")!;
}
