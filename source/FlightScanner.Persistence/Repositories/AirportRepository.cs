using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Exceptions;
using FlightScanner.Persistence.Database;
using FlightsScanner.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlightScanner.Persistence.Repositories;

public class AirportRepository : IAirportRepository
{
    private readonly AirportsDbContext _airportsDbContext;
    private readonly ILogger<AirportRepository> _logger;

    public AirportRepository(AirportsDbContext airportsDbContext, ILogger<AirportRepository> logger)
    {
        _airportsDbContext = airportsDbContext;
        _logger = logger;
    }

    public async Task<AirportEntity> GetAirportWithIataCode(string iataCode, CancellationToken cancellationToken)
    {
        var airport = await _airportsDbContext
            .Airports
            .AsNoTracking()
            .AsQueryable()
            .FirstOrDefaultAsync(
                predicate: airport => string.Equals(airport.IataCode, iataCode.ToUpper()),
                cancellationToken: cancellationToken);

        if (airport == null)
        {
            throw new NotFoundException(typeof(AirportEntity), iataCode);
        }

        _logger.LogInformation("Airport {iataCode} successfully fetched from database.", airport.IataCode);

        return airport;
    }

    public async Task<IReadOnlyList<AirportEntity>> GetAllAirports(CancellationToken cancellationToken)
    {
        var airports = await _airportsDbContext
            .Airports
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (airports == null)
        {
            throw new NotFoundException(typeof(AirportEntity), "Airports");
        }

        _logger.LogInformation("All airports successfully fetched from database.");

        return airports;
    }
}
