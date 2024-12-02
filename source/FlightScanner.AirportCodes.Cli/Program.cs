using CommandLine;
using FlightScanner.AirportCodes.Cli.Database;
using FlightScanner.AirportCodes.Cli.HtmlParser;
using FlightScanner.AirportCodes.Cli.Logging;

namespace FlightScanner.AirportCodes.Cli;

public class Program
{
    private static readonly char[] s_englishAlphabetCharacters = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];

    #region CLI dependencies

    private static readonly IDatabaseManager s_databaseManager = new SqlLiteDatabaseManager();

    private static readonly IHtmlDocumentParser s_htmlDocumentParser = new HtmlDocumentParser(new HttpClient());

    private static readonly ILogger s_logger = new CLIConsoleLogger();

    #endregion

    public static async Task Main(string[] args)
    {
        var parser = new Parser(with => with.HelpWriter = null);

        CommandLineOptions? inputOptions = null;
        IEnumerable<Error>? parsingErrors = null;
        var parserResult = parser.ParseArguments<CommandLineOptions>(args)
            .WithNotParsed(errors => parsingErrors = errors)
            .WithParsed(options => inputOptions = options);

        if (parsingErrors != null && parsingErrors.IsHelp() == false)
        {
            s_logger.LogErrorString("Error: Invalid arguments");
            return;
        }

        if (inputOptions == null)
        {
            s_logger.LogErrorString("Error: No input options");
            return;
        }

        char[] characters;
        if (string.IsNullOrEmpty(parserResult.Value.CharactersToParse))
        {
            characters = s_englishAlphabetCharacters;
        }
        else
        {
            characters = parserResult.Value.CharactersToParse.ToCharArray();
        }

        var airportCodeScrapper = new AirportCodesScrapper(
            databaseManager: s_databaseManager,
            htmlDocumentParser: s_htmlDocumentParser,
            logger: s_logger);

        await airportCodeScrapper.ParseCharacterCodesToAirportDatabase(parserResult.Value.ConnectionString, characters);
    }
}
