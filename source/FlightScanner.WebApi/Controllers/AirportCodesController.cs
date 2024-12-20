﻿using System.Net.Mime;
using FlightScanner.Domain.Entities;
using FlightScanner.DTOs.Exceptions;
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

    public AirportCodesController(ISender sender)
    {
        _sender = sender;
    }

    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AirportEntity))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionDto))]
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
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AllAirportsResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionDto))]
    [HttpGet]
    [Route("all-airports")]
    public async Task<IActionResult> GetAllAirport(
        CancellationToken cancellationToken)
    {
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
