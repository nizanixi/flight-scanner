using FlightScanner.Infrastructure.Configurations;

namespace FlightScanner.WebApi.Configurations;

public interface IWebApiConfiguration
{
    DatabaseConfiguration DatabaseConfiguration { get; }

    AmadeusEndpointConfiguration AmadeusEndpointConfiguration { get; }
}
