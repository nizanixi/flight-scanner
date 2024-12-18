using Microsoft.Extensions.Configuration;

namespace FlightsScanner.Application.Configurations;

public class AviationEndpointConfiguration
{
    private readonly IConfigurationSection _configurationSection;

    public AviationEndpointConfiguration(IConfigurationSection configurationSection, string aviationFlightSearchApiKey)
    {
        _configurationSection = configurationSection;
        AviationFlightSearchApiKey = aviationFlightSearchApiKey;
    }

    public string BaseUri => _configurationSection.GetValue<string>("BaseUri")!;

    public string AviationFlightSearchApiKey { get; }

    public string AccessKeyHeader => _configurationSection.GetValue<string>("AccessKeyHeader")!;

    public string DepartureIataCodeHeader => _configurationSection.GetValue<string>("DepartureIataCodeHeader")!;

    public string ArrivalIataCodeHeader => _configurationSection.GetValue<string>("ArrivalIataCodeHeader")!;

    public string FlightDateHeader => _configurationSection.GetValue<string>("FlightDateHeader")!;
}
