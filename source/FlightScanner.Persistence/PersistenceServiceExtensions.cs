using FlightScanner.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FlightScanner.Persistence;

public static class PersistenceServiceExtensions
{
    private const string IATA_AIRPORT_CODES_DATABASE_RELATIVE_LOCATION = "Data\\IataAirportCodesDatabase";

    private static readonly string s_databaseFilePath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        IATA_AIRPORT_CODES_DATABASE_RELATIVE_LOCATION);

    public static IServiceCollection AddInfrastructureProjectServices(this IServiceCollection services)
    {
        services.AddDbContext<AirportsDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseSqlite($"Data Source={s_databaseFilePath}");
        });

        return services;
    }
}
