using FlightScanner.AirportCodes.Cli.Models;
using HtmlAgilityPack;

namespace FlightScanner.AirportCodes.Cli.HtmlParser;

public class HtmlDocumentParser : IHtmlDocumentParser
{
    private const string WIKIPEDIA_PAGE_FOR_IATA_AIRPORT_CODES = "https://en.wikipedia.org/wiki/List_of_airports_by_IATA_airport_code:_";

    private readonly HttpClient _httpClient;

    public HtmlDocumentParser(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Airport>> ScrapeAirportsForLetter(char letter)
    {
        var url = $"{WIKIPEDIA_PAGE_FOR_IATA_AIRPORT_CODES}{letter}";

        var response = await _httpClient.GetStringAsync(url);

        return ParseHtmlDocument(response);
    }

    private static List<Airport> ParseHtmlDocument(string response)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(response);

        var tableRows = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable')]//tr");

        var airportData = new List<Airport>();
        if (tableRows != null)
        {
            foreach (var row in tableRows)
            {
                var cells = row.SelectNodes("td");
                if (cells != null && cells.Count >= 3)
                {
                    var iataCode = cells[0].InnerText.Trim();
                    var airportName = cells[1].InnerText.Trim();
                    var location = cells[2].InnerText.Trim();

                    var airport = new Airport(iataCode, airportName, location);

                    airportData.Add(airport);
                }
            }
        }

        return airportData;
    }
}

