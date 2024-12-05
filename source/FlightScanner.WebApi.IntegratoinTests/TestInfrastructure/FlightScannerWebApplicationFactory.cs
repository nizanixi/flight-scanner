using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace FlightScanner.WebApi.IntegratoinTests.TestInfrastructure;

public class FlightScannerWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// This will be invoked by the WebApplicationFactory when it’s time to bootstrap our API in memory.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Here we register all services that will be used exclusively during our tests or
        // replace any service with a test version like e.g. database service.
        builder.ConfigureTestServices(serviceCollection =>
        {

        });
    }
}
