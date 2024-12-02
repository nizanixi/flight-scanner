using FlightsScanner.Application.Constants;
using FlightsScanner.Application.Infrastructure;
using FlightsScanner.Application.Services.Contracts;
using FlightScanner.Persistence;
using FlightsScanner.Application.Airports.Queries.GetAirport;

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

        builder.Services.AddControllers();

        builder.Services.AddInfrastructureProjectServices();

        builder.Services.AddMemoryCache(options =>
        {
            options.SizeLimit = CacheConstants.CACHE_LIMIT;
        });
        builder.Services.AddScoped<IInMemoryCacheService, InMemoryCacheService>();

        builder.Services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(typeof(GetAirportQuery).Assembly);
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
    }
}
