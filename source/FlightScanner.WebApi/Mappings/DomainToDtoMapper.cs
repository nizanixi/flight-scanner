using FlightScanner.Domain.Entities;
using FlightScanner.Domain.Models;
using FlightScanner.DTOs.Models;

namespace FlightScanner.WebApi.Mappings;

public static class DomainToDtoMapper
{
    public static AirportDto MapToAirportDto(this AirportEntity airportEntity)
    {
        return new AirportDto(
            iataCode: airportEntity.IataCode,
            airportName: airportEntity.AirportName,
            location: airportEntity.Location);
    }

    public static FlightEntityDto MapToFlightDto(this FlightInformation flightEntityDto)
    {
        return new FlightEntityDto(
            departureAirportIataCode: flightEntityDto.DepartureAirportIataCode,
            departureDate: flightEntityDto.DepartureDate,
            arrivalAirportIataCode: flightEntityDto.ArrivalAirportIataCode,
            returnDate: flightEntityDto.ReturnDate,
            numberOfStops: flightEntityDto.NumberOfStops,
            numberOfBookableSeats: flightEntityDto.NumberOfBookableSeats,
            currency: flightEntityDto.Currency,
            price: flightEntityDto.Price)
        {
            DepartureAirportLocation = flightEntityDto.DepartureAirportLocation,
            ArrivalAirportLocation = flightEntityDto.ArrivalAirportLocation
        };
    }
}
