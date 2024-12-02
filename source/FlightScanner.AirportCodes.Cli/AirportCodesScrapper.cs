using FlightScanner.AirportCodes.Cli.Database;
using FlightScanner.AirportCodes.Cli.HtmlParser;
using FlightScanner.AirportCodes.Cli.Logging;
using FlightScanner.AirportCodes.Cli.Models;

namespace FlightScanner.AirportCodes.Cli;

public class AirportCodesScrapper
{
    private readonly IDatabaseManager _databaseManager;
    private readonly IHtmlDocumentParser _htmlDocumentParser;
    private readonly ILogger _logger;

    public AirportCodesScrapper(IDatabaseManager databaseManager, IHtmlDocumentParser htmlDocumentParser, ILogger logger)
    {
        _databaseManager = databaseManager;
        _htmlDocumentParser = htmlDocumentParser;
        _logger = logger;
    }

    public async Task ParseCharacterCodesToAirportDatabase(string connectionString, IReadOnlyList<char> characters)
    {
        _databaseManager.CreateDatabase(connectionString);

        var airportsForAllCharacters = new List<Airport>();
        foreach (var character in characters)
        {
            _logger.LogInfoString($"Scraping data for character: {character}...");

            var airportsForThisCharacter = await _htmlDocumentParser.ScrapeAirportsForLetter(character);

#if DEBUG
            var counter = 1;
            foreach (var airport in airportsForThisCharacter)
            {
                _logger.LogAirport(counter, airport);
                counter++;
            }
#endif

            _logger.LogInfoString($"Saving {airportsForThisCharacter.Count} records to the database...");

            airportsForAllCharacters.AddRange(airportsForThisCharacter);
        }

        _databaseManager.SaveToDatabase(connectionString, airportsForAllCharacters);

        _logger.LogInfoString("Data scraping and saving completed!");
    }
}
