﻿
using FlightScanner.Domain.Entities;

namespace FlightScanner.Domain.Repositories;

public interface IAirportRepository
{
    Task<AirportEntity> GetAirportWithIataCode(string iataCode, CancellationToken cancellationToken);

    Task<IReadOnlyList<AirportEntity>> GetAllAirports(CancellationToken cancellationToken);
}
