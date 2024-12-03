using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Infrastructure;
using FlightsScanner.Application.Services.Contracts;
using FlightScanner.Persistence;
using FlightsScanner.Application.Airports.Queries.GetAirport;
using FlightScanner.Domain.Services;
using FlightsScanner.Application.Services.Implementations;
using FlightScanner.Domain.Entities;
using FlightsScanner.Application.Flights.Queries.GetFlights;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        CreateWebBuilder(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        app.Run();
    }

    private static void CreateWebBuilder(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<Program>();

        builder.Services.AddControllers();

        builder.Services.AddInfrastructureProjectServices();

        builder.Services.AddMemoryCache(options =>
        {
            options.SizeLimit = CacheConstants.CACHE_LIMIT;
        });
        builder.Services.AddScoped<IInMemoryCacheService, InMemoryCacheService>();
        builder.Services.AddScoped<IFlightSearchService, AviationStackFlightSearchService>();

        builder.Services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(
                typeof(GetAirportQuery).Assembly,
                typeof(GetFlightsQuery).Assembly,
                typeof(FlightEntity).Assembly);
        });

        builder.Services.AddHttpClient(HttpClientConstants.AVIATIONSTACK_CLIENT_NAME);
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();
    }
}
