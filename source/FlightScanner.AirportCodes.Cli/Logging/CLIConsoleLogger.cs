using FlightScanner.AirportCodes.Cli.Models;

namespace FlightScanner.AirportCodes.Cli.Logging;

internal class CLIConsoleLogger : ILogger
{
    private readonly ConsoleColor _originalConsoleColor = ConsoleColor.White;

    public void LogInfoString(string info)
    {
        Console.WriteLine(info);
    }

    public void LogAirport(int counter, Airport airport)
    {
        Console.WriteLine($"{counter}.\t{airport.IATACode}-{airport.AirportName}\t{airport.Location}");
    }

    public void LogErrorString(string error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ForegroundColor = _originalConsoleColor;
    }
}
