using FluentValidation;

namespace FlightsScanner.Application.Airports.Queries.GetAllAirports;

public class GetAllAirportsQueryValidator : AbstractValidator<GetAllAirportsQuery>
{
    public GetAllAirportsQueryValidator()
    {
    }
}
