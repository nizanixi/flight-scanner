using FlightScanner.Domain.Exceptions;
using FlightScanner.Domain.Repositories;
using FlightScanner.DTOs.Models;
using FlightScanner.DTOs.Responses;
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
            .Select(i => new AirportDto(i.IataCode))
            .ToArray();

        return new AllAirportsResponseDto(airportResponseDtos);
    }
}
