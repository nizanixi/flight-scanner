using CommandLine;

namespace FlightScanner.AirportCodes.Cli;

public class CommandLineOptions
{
    [Value(index: 1, Required = true, HelpText = "Connection string for database file.", MetaName = "Connection string")]
    public string ConnectionString { get; set; } = null!;

    [Option('c', "characters", SetName = "characters", Default = null!, HelpText = "Use only specific alphabet characters.")]
    public string CharactersToParse { get; set; } = null!;
}
