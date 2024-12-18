using System.Net;
using System.Net.Http.Json;
using FlightScanner.Common.Constants;
using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Repositories;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;
using FlightScanner.WebApi.IntegratoinTests.TestInfrastructure;
using FlightsScanner.Application.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace FlightScanner.WebApi.IntegratoinTests.Controllers;

[TestFixture]
public class AvailableFlightsControllerTests
{
    [Test]
    public async Task GetFlightsOverHttp_WhenTwoRequestsAreSent_ShouldReceiveObjectFromRepositoryAndCachedObject()
    {
        var fakeAirportRepository = Substitute.For<IAirportRepository>();
        _ = fakeAirportRepository.GetAirportWithIataCode(
            iataCode: Arg.Any<string>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AirportEntity("", "", "")));
        var fakeFlightSearchService = Substitute.For<IFlightSearchService>();
        var flightEntity = new FlightEntityDto(
            departureAirportIataCode: string.Empty,
            departureDate: default,
            arrivalAirportIataCode: string.Empty,
            returnDate: default,
            numberOfStops: default,
            numberOfBookableSeats: default,
            currency: string.Empty,
            price: default);
        var expectedNumberOfFlights = 10;
        var flightsCollection = Enumerable.Range(1, expectedNumberOfFlights)
            .Select(_ => flightEntity)
            .ToList();
        var foundFlightsDto = new FoundFlightsResponseDto(flightsCollection);
        _ = fakeFlightSearchService.GetFlights(
            departureAirportIataCode: Arg.Any<string>(),
            departureTime: Arg.Any<DateTime>(),
            destinationAirportIataCode: Arg.Any<string>(),
            returnTripTime: Arg.Any<DateTime>(),
            numberOfPassengers: Arg.Any<int>(),
            currency: Arg.Any<string>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(foundFlightsDto);
        var applicationFactory = new FlightScannerWebApplicationFactory()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var flightSearchServiceDescriptor = services
                        .SingleOrDefault(d => d.ServiceType == typeof(IFlightSearchService))!;
                    services.Remove(flightSearchServiceDescriptor);
                    services.AddScoped(_ => fakeFlightSearchService);

                    var airportRepositoryServiceDescriptor = services
                        .SingleOrDefault(d => d.ServiceType == typeof(IAirportRepository))!;
                    services.Remove(airportRepositoryServiceDescriptor);
                    services.AddScoped(_ => fakeAirportRepository);
                });
            });
        var httpClient = applicationFactory.CreateClient();
        var flightSearchUri = CreateFlightSearchUri();
        var expectedNumberOfCallsToRepository = 1;

        var firstResponse = await httpClient.GetAsync(flightSearchUri);
        var flightsFoundOnFirstRequest = await firstResponse.Content.ReadFromJsonAsync<FoundFlightsResponseDto>();

        var secondResponse = await httpClient.GetAsync(flightSearchUri);
        var flightsFoundOnSecondRequest = await secondResponse.Content.ReadFromJsonAsync<FoundFlightsResponseDto>();

        var callInfo = fakeFlightSearchService.ReceivedCalls();

        Assert.Multiple(() =>
        {
            Assert.That(callInfo.Count, Is.EqualTo(expectedNumberOfCallsToRepository), "Caching doesn't work as expected!");

            Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(flightsFoundOnFirstRequest!.FlightEntityDtos.Count, Is.EqualTo(expectedNumberOfFlights));

            Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(flightsFoundOnSecondRequest!.FlightEntityDtos.Count, Is.EqualTo(expectedNumberOfFlights));
        });
    }

    #region Helper methods

    private static string CreateFlightSearchUri()
    {
        var departureDateTime = DateTime.Now
            .AddDays(1)
            .ToString(DateTimeConstants.DATE_TIME_FORMAT);

        var returnDateTime = DateTime.Now
            .AddDays(7)
            .ToString(DateTimeConstants.DATE_TIME_FORMAT);

        return $"api/v1/flights?departureAirportIataCode=AAA&departureTime={departureDateTime}&destinationAirportIataCode=SID&returnTripTime={returnDateTime}&numberOfPassengers=1&currency=HRK";
    }

    #endregion
}
