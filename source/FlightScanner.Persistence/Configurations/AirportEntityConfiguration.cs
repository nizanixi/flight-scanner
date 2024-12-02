using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlightScanner.Domain.Models;

namespace FlightScanner.Persistence.Configurations;

public class AirportEntityConfiguration : IEntityTypeConfiguration<Airport>
{
    public void Configure(EntityTypeBuilder<Airport> builder)
    {
        builder.HasKey(e => e.IataCode);
        builder.Property(e => e.IataCode)
            .HasColumnName("IATA_Code");

        builder.Property(e => e.AirportName)
            .HasColumnName("Airport_Name");

        builder.Property(e => e.Location)
            .HasColumnName("Location");
    }
}
