using FlightScanner.Common.Constants;
using FluentValidation;

namespace FlightsScanner.Application.Airports.Queries.GetAirport;

public class GetAirportQueryValidator : AbstractValidator<GetAirportQuery>
{
    public GetAirportQueryValidator()
    {
        RuleFor(airport => airport.IataCode)
            .Length(IataCodeConstants.IATA_CODE_LENGTH).WithMessage(airport => $"IATA code {airport.IataCode} should be exactly 3 characters long!");
    }
}
