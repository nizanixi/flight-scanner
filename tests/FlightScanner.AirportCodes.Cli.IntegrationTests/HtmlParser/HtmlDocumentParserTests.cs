namespace FlightScanner.AirportCodes.Cli.HtmlParser;

[TestFixture]
public class HtmlDocumentParserTests
{
    [Test]
    public async Task ScrapeAirportsForLetter_ForLetterA_ShouldParseExpectedNumberOfValues()
    {
        var htmlDocumentParser = new HtmlDocumentParser(new HttpClient());
        const int expectedNumberOfAirportsForLetterA = 530;

        var airportsForLetterA = await htmlDocumentParser.ScrapeAirportsForLetter('A');

        Assert.That(airportsForLetterA.Count, Is.GreaterThanOrEqualTo(expectedNumberOfAirportsForLetterA));
    }
}
