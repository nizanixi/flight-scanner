using FlightScanner.AirportCodes.Cli.Models;

namespace FlightScanner.AirportCodes.Cli.Logging;

public interface ILogger
{
    void LogInfoString(string info);

    void LogAirport(int counter, Airport airport);

    void LogErrorString(string error);
}
