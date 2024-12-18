using System.Net;
using System.Net.Http.Json;
using FlightScanner.Common.Constants;
using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Repositories;
using FlightScanner.DTOs.Responses;
using FlightScanner.WebApi.IntegratoinTests.TestInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace FlightScanner.WebApi.IntegratoinTests.Controllers;

[TestFixture]
public class AirportCodesControllerTests
{
    [Test]
    public async Task GetAirportOverHttp_WhenTwoRequestsAreSent_ShouldReceiveObjectFromRepositoryAndCachedObject()
    {
        var iataCode = "AAA";
        var fakeAirportRepository = Substitute.For<IAirportRepository>();
        var airportEntity = new AirportEntity(iataCode, "Airport name", "Location");
        fakeAirportRepository.GetAirportWithIataCode(iataCode, Arg.Any<CancellationToken>())
            .Returns(airportEntity);
        var applicationFactory = new FlightScannerWebApplicationFactory()
            .WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var airportRepositoryDescriptor = services
                    .SingleOrDefault(d => d.ServiceType == typeof(IAirportRepository))!;
                services.Remove(airportRepositoryDescriptor);
                services.AddScoped(_ => fakeAirportRepository);
            });
        });
        var httpClient = applicationFactory.CreateClient();
        var expectedNumberOfCallsToRepository = 1;

        var firstResponse = await httpClient.GetAsync($"api/v2/airport?iataCode={iataCode}");
        var airportFoundOnFirstRequest = await firstResponse.Content.ReadFromJsonAsync<AirportEntity>();

        var secondResponse = await httpClient.GetAsync($"api/v2/airport?iataCode={iataCode}");
        var airportFoundOnSecondRequest = await secondResponse.Content.ReadFromJsonAsync<AirportEntity>();

        var callInfo = fakeAirportRepository.ReceivedCalls();

        Assert.Multiple(() =>
        {
            Assert.That(callInfo.Count, Is.EqualTo(expectedNumberOfCallsToRepository));

            Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(airportEntity.IataCode, Is.EqualTo(airportFoundOnFirstRequest!.IataCode));

            Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(airportEntity.IataCode, Is.EqualTo(airportFoundOnSecondRequest!.IataCode));
        });
    }

    [Test]
    public async Task GetAirportOverHttp_WhenRequestContainsIataCodeWithInvalidLength_ShouldReceiveErrorStatusCode()
    {
        var iataCodeWithInvalidLength = "ABCD";
        var applicationFactory = new FlightScannerWebApplicationFactory();
        var httpClient = applicationFactory.CreateClient();
        var expectedErrorMessage = $"IATA code has invalid length of {iataCodeWithInvalidLength.Length}. IATA code should have {IataCodeConstants.IATA_CODE_LENGTH} text characters.";

        var foundMovieResponse = await httpClient.GetAsync($"api/v2/airport?iataCode={iataCodeWithInvalidLength}");
        var contentString = await foundMovieResponse.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(foundMovieResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.Contains(expectedErrorMessage, contentString);
        });
    }

    [Ignore("Test failing only on GitHub")]
    [Test]
    public async Task GetAirportOverHttp_WhenRequestContainsForbiddenIataCode_ShouldReceiveErrorStatusCode()
    {
        var closedAirport = "UCK";
        var applicationFactory = new FlightScannerWebApplicationFactory();
        var httpClient = applicationFactory.CreateClient();
        var expectedErrorTitle = "Forbidden IATA code";
        var expectedErrorMessage = $"Airport with IATA code {closedAirport} is currently closed due to war conditions!";

        var httpResponse = await httpClient.GetAsync($"api/v2/airport?iataCode={closedAirport}");
        var contentString = await httpResponse.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.Contains(expectedErrorTitle, contentString);
            StringAssert.Contains(expectedErrorMessage, contentString);
        });
    }

    [Ignore("Test failing only on GitHub")]
    [Test]
    public async Task GetAirportOverHttp_WhenRequestIsValid_ShouldReceiveExpectedResponse()
    {
        var searchedAirport = "BOS";
        var applicationFactory = new FlightScannerWebApplicationFactory();
        var httpClient = applicationFactory.CreateClient();

        var httpResponse = await httpClient.GetAsync($"api/v2/airport?iataCode={searchedAirport}");

        Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Ignore("Test failing only on GitHub")]
    [Test]
    public async Task GetAirportsOverHttp_WhenAllAirportsAreRequested_ShouldReceiveExpectedNumberOfAirports()
    {
        var fakeAirportRepository = Substitute.For<IAirportRepository>();
        var airportEntity = new AirportEntity("AAA", "Airport name", "Location");
        var expectedNumberOfAirports = 50;
        var airportsCollection = Enumerable.Range(1, expectedNumberOfAirports)
            .Select(_ => airportEntity)
            .ToList();
        fakeAirportRepository.GetAllAirports(Arg.Any<CancellationToken>())
            .Returns(airportsCollection);
        var applicationFactory = new FlightScannerWebApplicationFactory()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var airportRepositoryDescriptor = services
                        .SingleOrDefault(d => d.ServiceType == typeof(IAirportRepository))!;
                    services.Remove(airportRepositoryDescriptor);
                    services.AddScoped(_ => fakeAirportRepository);
                });
            });
        var httpClient = applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/v2/all-airports");
        var allAirports = await response.Content.ReadFromJsonAsync<AllAirportsResponseDto>();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(expectedNumberOfAirports, Is.EqualTo(allAirports!.Airports.Count));
        });
    }
}
