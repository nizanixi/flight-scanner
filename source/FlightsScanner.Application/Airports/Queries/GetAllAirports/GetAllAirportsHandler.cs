using FlightScanner.Domain.Exceptions;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;
using FlightsScanner.Application.Interfaces.Repositories;
using MediatR;

namespace FlightsScanner.Application.Airports.Queries.GetAllAirports;

public class GetAllAirportsHandler : IRequestHandler<GetAllAirportsQuery, AllAirportsResponseDto>
{
    private readonly IAirportRepository _airportRepository;

    public GetAllAirportsHandler(IAirportRepository airportRepository)
    {
        _airportRepository = airportRepository;
    }

    public async Task<AllAirportsResponseDto> Handle(GetAllAirportsQuery request, CancellationToken cancellationToken)
    {
        var airports = await _airportRepository.GetAllAirports(cancellationToken);

        if (airports == null || !airports.Any())
        {
            throw new InvalidResponseException($"There are not airports found!");
        }

        var airportResponseDtos = airports
            .Select(i => new AirportDto(i.IataCode, i.AirportName, i.Location))
            .ToArray();

        return new AllAirportsResponseDto(airportResponseDtos);
    }
}
