using System.Net.Mime;
using FlightScanner.DTOs.Exceptions;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;
using FlightScanner.WebApi.Filters;
using FlightScanner.WebApi.Mappings;
using FlightScanner.WebApi.Validation;
using FlightsScanner.Application.Airports.Queries.GetAirport;
using FlightsScanner.Application.Airports.Queries.GetAllAirports;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlightScanner.WebApi.Controllers;

[ApiController]
[Route("api/v2")]
[AirportCodesVersionDiscontinuationResourceFilter]
public class AirportCodesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<AirportCodesController> _logger;

    public AirportCodesController(ISender sender, ILogger<AirportCodesController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AirportDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionDto))]
    [HttpGet]
    [Route("airport")]
    [CountriesWithClosedAirTrafficFilter]
    public async Task<IActionResult> GetAirport(
        [FromQuery][IataCodeValidation] string iataCode,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("HTTP request for getting airport with IATA code {iataCode}", iataCode);

        var foundAirport = await _sender.Send(
            request: new GetAirportQuery(iataCode),
            cancellationToken: cancellationToken);

        var airportDto = foundAirport.MapToAirportDto();

        return Ok(airportDto);
    }

    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AllAirportsResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionDto))]
    [HttpGet]
    [Route("all-airports")]
    public async Task<IActionResult> GetAllAirport(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("HTTP request for getting all airports");

        var airportEntities = await _sender.Send(
            request: new GetAllAirportsQuery(),
            cancellationToken: cancellationToken);

        var airportDtos = airportEntities
            .Select(DomainToDtoMapper.MapToAirportDto)
            .ToArray();

        var airportsResponse = new AllAirportsResponseDto(airportDtos);

        return Ok(airportsResponse);
    }
}
