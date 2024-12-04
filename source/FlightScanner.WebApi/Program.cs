using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Services.Contracts;
using FlightScanner.Persistence;
using FlightsScanner.Application.Airports.Queries.GetAirport;
using FlightsScanner.Application.Services.Implementations;
using FlightsScanner.Application.Flights.Queries.GetFlights;
using FlightsScanner.Application.Authentication;
using FlightScanner.WebApi.Middleware;
using FlightScanner.DTOs.Models;

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

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        builder.Services.AddControllers();

        builder.Services.AddInfrastructureProjectServices();

        builder.Services.AddMemoryCache(options =>
        {
            options.SizeLimit = CacheConstants.CACHE_LIMIT;
        });
        builder.Services.AddScoped<IFlightSearchService, AmadeusFlightSearchService>();
        builder.Services.AddScoped<IAmadeusAuthorizatoinHandlerService, AmadeusAuthorizatoinHandlerService>();

        builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

        builder.Services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(
                typeof(GetAirportQuery).Assembly,
                typeof(GetFlightsQuery).Assembly,
                typeof(FlightEntityDto).Assembly);
        });

        builder.Services.AddHttpClient(HttpClientConstants.DEFAULT_HTTP_CLIENT_NAME);
        builder.Services.AddHttpClient(HttpClientConstants.AMADEUS_CLIENT_NAME)
            .AddHttpMessageHandler(provider =>
            {
                var amadeusAuthorizatoinHandlerService = provider.GetRequiredService<IAmadeusAuthorizatoinHandlerService>();

                return new AuthenticatedHttpClientHandler(amadeusAuthorizatoinHandlerService);
            });
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

        app.UseCors();

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
