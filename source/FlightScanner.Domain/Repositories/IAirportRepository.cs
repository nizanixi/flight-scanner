
using FlightScanner.Domain.Entities;

namespace FlightScanner.Domain.Repositories;

public interface IAirportRepository
{
    Task<AirportEntity> GetAirportWithIataCode(string iataCode, CancellationToken cancellationToken);
}
