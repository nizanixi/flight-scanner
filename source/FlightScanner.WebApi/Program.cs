using System.Reflection;
using FlightScanner.Common.Constants;
using FlightScanner.Domain.Repositories;
using FlightScanner.DTOs.Models;
using FlightScanner.Persistence.Database;
using FlightScanner.Persistence.Repositories;
using FlightScanner.WebApi.Configurations;
using FlightScanner.WebApi.Middleware;
using FlightsScanner.Application.Airports.Queries.GetAirport;
using FlightsScanner.Application.Authentication;
using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Flights.Queries.GetFlights;
using FlightsScanner.Application.Services.Contracts;
using FlightsScanner.Application.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class Program
{
    private const string CORS_POLICY_NAME = "BlazorClientFrontend";

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
        builder.Services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
        });

        builder.Services.AddSingleton<IWebApiConfiguration, WebApiConfiguration>();

        var environmentName = Environment.GetEnvironmentVariable(EnvironmentVariableNamesConstants.ASP_NET_CORE_ENVIRONMENT) ?? string.Empty;

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile(path: $"appsettings.{environmentName}.json", optional: true)
            .AddUserSecrets<Program>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CORS_POLICY_NAME, builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        builder.Services.AddControllers();

        AddPersistence(builder.Services);

        builder.Services.AddMemoryCache(options =>
        {
            options.SizeLimit = CacheConstants.CACHE_LIMIT;
        });
        builder.Services.AddScoped<IFlightSearchService, AmadeusFlightSearchService>(sp =>
        {
            var httpClientFactory = sp.GetService<IHttpClientFactory>()!;
            var amadeusEndpointConfiguration = sp.GetService<IWebApiConfiguration>()!.AmadeusEndpointConfiguration;

            return new AmadeusFlightSearchService(httpClientFactory, amadeusEndpointConfiguration);
        });
        builder.Services.AddScoped<IAmadeusAuthorizatoinHandlerService, AmadeusAuthorizatoinHandlerService>(sp =>
        {
            var httpClientFactory = sp.GetService<IHttpClientFactory>()!;
            var amadeusEndpointConfiguration = sp.GetService<IWebApiConfiguration>()!.AmadeusEndpointConfiguration;
            var memoryCache = sp.GetService<IMemoryCache>()!;

            return new AmadeusAuthorizatoinHandlerService(httpClientFactory, amadeusEndpointConfiguration, memoryCache);
        });

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

        app.UseCors(CORS_POLICY_NAME);

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }

    private static void AddPersistence(IServiceCollection services)
    {
        services.AddScoped<IAirportRepository, AirportRepository>();

        services.AddDbContext<AirportsDbContext>((serviceProvider, optionsBuilder) =>
        {
            var configuration = serviceProvider.GetRequiredService<IWebApiConfiguration>();

            var databaseFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                configuration.DatabaseConfiguration.DatabaseFolder,
                configuration.DatabaseConfiguration.DatabaseName);

            optionsBuilder.UseSqlite(
                connectionString: $"Data Source={databaseFilePath}",
                sqlOptionsAction =>
                {
                    sqlOptionsAction.CommandTimeout(configuration.DatabaseConfiguration.CommandTimeoutInSeconds);
                });

            optionsBuilder.EnableDetailedErrors(configuration.DatabaseConfiguration.EnableDetailedErrors);
            optionsBuilder.EnableSensitiveDataLogging(configuration.DatabaseConfiguration.EnableSensitiveDataLogging);
            optionsBuilder.UseQueryTrackingBehavior(configuration.DatabaseConfiguration.QueryTrackingBehavior);
        });
    }
}
