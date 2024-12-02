﻿using FlightScanner.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightScanner.Persistence.Database;

public class AirportsDbContext : DbContext
{
    public AirportsDbContext(DbContextOptions<AirportsDbContext> options)
        : base(options)
    {
    }

    #region Properties

    public DbSet<Airport> Airports { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            assembly: typeof(AirportsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
