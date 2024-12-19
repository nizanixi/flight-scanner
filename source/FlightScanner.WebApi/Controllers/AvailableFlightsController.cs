using System.Net.Mime;
using FlightScanner.Common.Constants;
using FlightScanner.DTOs.Exceptions;
using FlightScanner.DTOs.Responses;
using FlightScanner.WebApi.Mappings;
using FlightScanner.WebApi.Validation;
using FlightsScanner.Application.Flights.Queries.GetFlights;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FlightScanner.WebApi.Controllers;

[ApiController]
[Route("api/v1")]
public class AvailableFlightsController : ControllerBase
{
    private readonly ISender _sender;

    public AvailableFlightsController(ISender sender)
    {
        _sender = sender;
    }

    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FoundFlightsResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionDto))]
    [HttpGet]
    [Route("flights")]
    public async Task<IActionResult> GetAvailableFlights(
        [FromQuery][IataCodeValidation] string departureAirportIataCode,
        [FromQuery][DepartureDateTimeValidation]
        [SwaggerParameter($"Date time format should be as following: {DateTimeConstants.DATE_TIME_FORMAT}")]
        string departureTime,
        [FromQuery][IataCodeValidation] string destinationAirportIataCode,
        [FromQuery][ArrivalDateTimeValidation]
        [SwaggerParameter($"Date time format should be as following: {DateTimeConstants.DATE_TIME_FORMAT}")]
        string returnTripTime,
        [FromQuery][PositiveNumberValidation] int numberOfPassengers,
        [FromQuery][CurrencyValidation] string currency,
        CancellationToken cancellationToken)
    {
        var getFlightsQuery = new GetFlightsQuery(
            departureAirportIataCode: departureAirportIataCode,
            departureTime: DateTime.ParseExact(departureTime, DateTimeConstants.DATE_TIME_FORMAT, null),
            destinationAirportIataCode: destinationAirportIataCode,
            returnTripTime: DateTime.ParseExact(returnTripTime, DateTimeConstants.DATE_TIME_FORMAT, null),
            numberOfPassengers: numberOfPassengers,
            currency: currency);

        var flightOfferInformations = await _sender.Send(
            request: getFlightsQuery,
            cancellationToken: cancellationToken);

        var flightOfferDtos = flightOfferInformations
            .Select(DomainToDtoMapper.MapToFlightDto)
            .ToArray();

        var flightOffers = new FoundFlightsResponseDto(flightOfferDtos);

        return Ok(flightOffers);
    }
}
