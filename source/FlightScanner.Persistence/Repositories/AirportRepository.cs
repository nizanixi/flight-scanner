using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Exceptions;
using FlightScanner.Domain.Repositories;
using FlightScanner.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace FlightScanner.Persistence.Repositories;

public class AirportRepository : IAirportRepository
{
    private readonly AirportsDbContext _airportsDbContext;

    public AirportRepository(AirportsDbContext airportsDbContext)
    {
        _airportsDbContext = airportsDbContext;
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

        return airport;
    }

    public async Task<IReadOnlyList<AirportEntity>> GetAllAirports(CancellationToken cancellationToken)
    {
        var airports = await _airportsDbContext
            .Airports
            .AsNoTracking()
            .ToListAsync();

        if (airports == null)
        {
            throw new NotFoundException(typeof(AirportEntity), "Airports");
        }

        return airports;
    }
}
