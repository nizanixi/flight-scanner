using FlightScanner.AirportCodes.Cli.Models;

namespace FlightScanner.AirportCodes.Cli.HtmlParser;

public interface IHtmlDocumentParser
{
    Task<List<Airport>> ScrapeAirportsForLetter(char letter);
}
