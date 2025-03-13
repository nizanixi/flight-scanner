using FlightScanner.Client.BlazorWA.Models;
using FlightScanner.Common.Constants;
using FluentValidation;

namespace FlightScanner.Client.BlazorWA.Validation;

public class FlightSearchViewModelValidator : AbstractValidator<FlightSearchViewModel>
{
    public FlightSearchViewModelValidator()
    {
        RuleFor(flight => flight.DepartureAirportIataCode)
            .Length(IataCodeConstants.IATA_CODE_LENGTH).WithMessage("IATA code should be exactly 3 characters long!")
            .Must((flightEntity, departureIataCode) => !string.Equals(flightEntity.DestionationAirportIataCode, departureIataCode, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Departure IATA code and destination IATA code can't be same!");

        RuleFor(flight => flight.DepartureDate)
            .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("Departure date cannot be in the past!")
            .LessThan(flight => flight.ReturnDate).WithMessage("Departure date cannot be after return date!");

        RuleFor(flight => flight.DestionationAirportIataCode)
            .Length(IataCodeConstants.IATA_CODE_LENGTH).WithMessage("IATA code should be exactly 3 characters long!")
            .Must((flightEntity, arrivalIataCode) => !string.Equals(flightEntity.DepartureAirportIataCode, arrivalIataCode, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Departure IATA code and destination IATA code can't be same!");

        RuleFor(flight => flight.ReturnDate)
            .GreaterThan(flight => flight.DepartureDate).WithMessage("Return date must be after departure date!");

        RuleFor(flight => flight.NumberOfPassengers)
            .GreaterThan(0).WithMessage("Number of passengers must be at least 1.")
            .LessThanOrEqualTo(9).WithMessage("Maximum number of bookable seats is 9.");

        RuleFor(flight => flight.SelectedCurrency)
            .NotEmpty().WithMessage("Currency can't be empty!");
    }
}
