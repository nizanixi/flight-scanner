using FlightScanner.Domain.Entities;
using FlightScanner.WebApi.Filters;
using FlightScanner.WebApi.Validation;
using FlightsScanner.Application.Airports.Queries.GetAirport;
using FlightsScanner.Application.Airports.Queries.GetAllAirports;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FlightScanner.WebApi.Controllers;

[ApiController]
[Route("api/v2")]
[AirportCodesVersionDiscontinuationResourceFilter]
public class AirportCodesController : ControllerBase
{
    private readonly ISender _sender;

    public AirportCodesController(ISender sender)
    {
        _sender = sender;
    }

    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AirportEntity))]
    [HttpGet]
    [Route("airport")]
    [CountriesWithClosedAirTrafficFilter]
    public async Task<IActionResult> GetAirport(
        [FromQuery][IataCodeValidation] string iataCode,
        CancellationToken cancellationToken)
    {
        var foundAirport = await _sender.Send(
            request: new GetAirportQuery(iataCode),
            cancellationToken: cancellationToken);

        return Ok(foundAirport);
    }

    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IReadOnlyList<AirportEntity>))]
    [HttpGet]
    [Route("all-airports")]
    public async Task<IActionResult> GetAllAirport(
        CancellationToken cancellationToken)
    {
        var airportsResponse = await _sender.Send(
            request: new GetAllAirportsQuery(),
            cancellationToken: cancellationToken);

        return Ok(airportsResponse);
    }
}
