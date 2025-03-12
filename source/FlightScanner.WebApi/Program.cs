using System.Reflection;
using FlightScanner.Common.Constants;
using FlightScanner.Common.Policies;
using FlightScanner.DTOs.Models;
using FlightScanner.Infrastructure.Authorization;
using FlightScanner.Infrastructure.Constants;
using FlightScanner.Infrastructure.HttpClients;
using FlightScanner.Persistence.Database;
using FlightScanner.Persistence.Repositories;
using FlightScanner.WebApi.Configurations;
using FlightScanner.WebApi.Middleware;
using FlightsScanner.Application.Airports.Queries.GetAirport;
using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Flights.Queries.GetFlights;
using FlightsScanner.Application.Interfaces.HttpClients;
using FlightsScanner.Application.Interfaces.Repositories;
using FlightsScanner.Application.PipelineBehaviors;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Configuration;
using Serilog;

public class Program
{
    private const string CORS_POLICY_NAME = "BlazorClientFrontend";
    private const int HTTP_RETRY_IN_SECONDS = 20;
    private const int HTTP_TIMEOUT_IN_SECONDS = 60;

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

        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSerilog();

            loggingBuilder.AddConfiguration();
        });

        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });

        builder.Services.AddValidatorsFromAssemblies([
            typeof(GetAirportQueryValidator).Assembly]);
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));

        AddPersistence(builder.Services);

        builder.Services.AddMemoryCache(options =>
        {
            options.SizeLimit = CacheConstants.CACHE_LIMIT;
        });
        builder.Services.AddScoped<IFlightSearchHttpClient, AmadeusFlightSearchHttpClient>(sp =>
        {
            var httpClientFactory = sp.GetService<IHttpClientFactory>()!;
            var amadeusEndpointConfiguration = sp.GetService<IWebApiConfiguration>()!.AmadeusEndpointConfiguration;

            return new AmadeusFlightSearchHttpClient(httpClientFactory, amadeusEndpointConfiguration);
        });
        builder.Services.AddScoped<IAmadeusAuthorizatoinHttpClient, AmadeusAuthorizatoinHttpClient>(sp =>
        {
            var httpClientFactory = sp.GetService<IHttpClientFactory>()!;
            var amadeusEndpointConfiguration = sp.GetService<IWebApiConfiguration>()!.AmadeusEndpointConfiguration;
            var memoryCache = sp.GetService<IMemoryCache>()!;

            return new AmadeusAuthorizatoinHttpClient(httpClientFactory, amadeusEndpointConfiguration, memoryCache);
        });

        builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

        builder.Services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(
                typeof(GetAirportQuery).Assembly,
                typeof(GetFlightsQuery).Assembly,
                typeof(FlightEntityDto).Assembly);
        });

        builder.Services.AddHttpClient(HttpClientConstants.DEFAULT_HTTP_CLIENT_NAME)
            .ConfigureHttpClient(provider =>
            {
                provider.Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_IN_SECONDS);
            })
            .AddPolicyHandler(HttpPoliciesFactory.CreateRetryPolicyWithSameRetryTime(HTTP_RETRY_IN_SECONDS))
            .AddPolicyHandler(HttpPoliciesFactory.CreateCircuitBreakingPolicyForTooManyRequests());
        builder.Services.AddHttpClient(HttpClientConstants.AMADEUS_CLIENT_NAME)
            .ConfigureHttpClient(provider =>
            {
                provider.Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_IN_SECONDS);
            })
            .AddHttpMessageHandler(provider =>
            {
                var amadeusAuthorizatoinHttpClient = provider.GetRequiredService<IAmadeusAuthorizatoinHttpClient>();

                return new AuthorizationHttpClientHandler(amadeusAuthorizatoinHttpClient);
            })
            .AddPolicyHandler(HttpPoliciesFactory.CreateRetryPolicyWithSameRetryTime(HTTP_RETRY_IN_SECONDS));
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
