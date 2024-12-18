using Microsoft.Extensions.Configuration;

namespace FlightsScanner.Application.Configurations;

public class AmadeusEndpointConfiguration
{
    private readonly IConfigurationSection _configurationSection;

    public AmadeusEndpointConfiguration(IConfigurationSection configurationSection, string amadeusFlightSearchApiKey, string amadeusFlightSearchApiSecret)
    {
        _configurationSection = configurationSection;
        AmadeusFlightSearchApiKey = amadeusFlightSearchApiKey;
        AmadeusFlightSearchApiSecret = amadeusFlightSearchApiSecret;
    }

    public string BaseUri => _configurationSection.GetValue<string>("BaseUri")!;

    #region Endpoints

    public string GetFlightEndpoint => _configurationSection.GetValue<string>("GetFlightEndpoint")!;

    public string AuthenticationEndpoint => _configurationSection.GetValue<string>("AuthenticationEndpoint")!;

    #endregion

    public string AmadeusFlightSearchApiKey { get; }

    public string AmadeusFlightSearchApiSecret { get; }

    public string GrantTypeHeader => _configurationSection.GetValue<string>("GrantTypeHeader")!;

    public string ClientCredentialsGrantType => _configurationSection.GetValue<string>("ClientCredentialsGrantType")!;

    public string ClientIdHeader => _configurationSection.GetValue<string>("ClientIdHeader")!;

    public string ClientSecretHeader => _configurationSection.GetValue<string>("ClientSecretHeader")!;

    #region Query strings

    public string OriginLocationCodeQueryString => _configurationSection.GetValue<string>("OriginLocationCodeQueryString")!;

    public string DepartureDateQueryString => _configurationSection.GetValue<string>("DepartureDateQueryString")!;

    public string DestinationLocationCodeQueryString => _configurationSection.GetValue<string>("DestinationLocationCodeQueryString")!;

    public string ReturnDateQueryString => _configurationSection.GetValue<string>("ReturnDateQueryString")!;

    public string AdultsQueryString => _configurationSection.GetValue<string>("AdultsQueryString")!;

    public string CurrencyCodeQueryString => _configurationSection.GetValue<string>("CurrencyCodeQueryString")!;

    #endregion
}
