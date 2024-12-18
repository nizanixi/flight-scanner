using FlightsScanner.Application.Configurations;

namespace FlightScanner.WebApi.Configurations;

public class WebApiConfiguration : IWebApiConfiguration
{
    public WebApiConfiguration(IConfiguration configuration)
    {
        DatabaseConfiguration = new DatabaseConfiguration(
            configurationSection: configuration.GetSection("DatabaseConfiguration"));

        AmadeusEndpointConfiguration = new AmadeusEndpointConfiguration(
            configurationSection: configuration.GetSection("AmadeusEndpointConfiguration"),
            amadeusFlightSearchApiKey: configuration["AmadeusFlightSearchApiKey"]!,
            amadeusFlightSearchApiSecret: configuration["AmadeusFlightSearchApiSecret"]!);
    }

    public DatabaseConfiguration DatabaseConfiguration { get; }

    public AmadeusEndpointConfiguration AmadeusEndpointConfiguration { get; }
}
