namespace FlightScanner.Client.BlazorWA.Configurations;

public class FlightsEndpointConfiguration
{
    private readonly IConfigurationSection _configurationSection;

    public FlightsEndpointConfiguration(IConfigurationSection configurationSection)
    {
        _configurationSection = configurationSection;
    }

    public string GetFlightEndpoint => _configurationSection.GetValue<string>("GetFlightEndpoint")!;

    public string DepartureAirportQueryString => _configurationSection.GetValue<string>("DepartureAirportQueryString")!;

    public string DepartureTimeQueryString => _configurationSection.GetValue<string>("DepartureTimeQueryString")!;

    public string DestinationAirportQueryString => _configurationSection.GetValue<string>("DestinationAirportQueryString")!;

    public string ReturnTimeQueryString => _configurationSection.GetValue<string>("ReturnTimeQueryString")!;

    public string NumberOfPassengersQueryString => _configurationSection.GetValue<string>("NumberOfPassengersQueryString")!;

    public string CurrencyQueryString => _configurationSection.GetValue<string>("CurrencyQueryString")!;
}
