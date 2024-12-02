using FlightScanner.AirportCodes.Cli.Models;

namespace FlightScanner.AirportCodes.Cli.Database;

public interface IDatabaseManager
{
    void CreateDatabase(string connectionString);

    void SaveToDatabase(string connectionString, List<Airport> airportData);
}
