using FlightScanner.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlightScanner.Persistence.DesignTime;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AirportsDbContext>
{
    public AirportsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AirportsDbContext>();
        optionsBuilder.UseSqlite("Data Source=airports.db");

        return new AirportsDbContext(optionsBuilder.Options);
    }
}
