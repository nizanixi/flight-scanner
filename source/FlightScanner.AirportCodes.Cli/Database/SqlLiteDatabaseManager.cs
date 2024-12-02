using FlightScanner.AirportCodes.Cli.Models;
using System.Data.SQLite;

namespace FlightScanner.AirportCodes.Cli.Database;

public class SqlLiteDatabaseManager : IDatabaseManager
{
    public void CreateDatabase(string connectionString)
    {
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();

        var createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Airports (
                        IATA_Code TEXT PRIMARY KEY,
                        Airport_Name TEXT,
                        Location TEXT
                    );
                ";

        using var command = new SQLiteCommand(createTableQuery, connection);
        command.ExecuteNonQuery();
    }

    public void SaveToDatabase(string connectionString, List<Airport> airportData)
    {
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();

        var insertQuery = @"
                    INSERT OR IGNORE INTO Airports (IATA_Code, Airport_Name, Location)
                    VALUES (@IATA_Code, @Airport_Name, @Location);
                ";

        foreach (var airport in airportData)
        {
            using var command = new SQLiteCommand(insertQuery, connection);

            command.Parameters.AddWithValue("@IATA_Code", airport.IATACode);
            command.Parameters.AddWithValue("@Airport_Name", airport.AirportName);
            command.Parameters.AddWithValue("@Location", airport.Location);

            command.ExecuteNonQuery();
        }
    }
}
