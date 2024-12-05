using FlightScanner.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlightScanner.WebApi.Filters;

public class CountriesWithClosedAirTrafficFilter : ActionFilterAttribute
{
    private readonly string[] _ukrainianAirportIataCodes = new string[]
    {
        "UCK",
        "UDJ",
        "UMY",
    };

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);

        if (context.Result is not OkObjectResult result)
        {
            return;
        }

        if (result.Value is not AirportEntity airport)
        {
            return;
        }

        if (_ukrainianAirportIataCodes.Any(ukrainianAirportIataCode => string.Equals(ukrainianAirportIataCode, airport.IataCode, StringComparison.OrdinalIgnoreCase)))
        {
            context.ModelState.AddModelError(
                key: "Forbidden IATA code",
                errorMessage: $"Airport with IATA code {airport.IataCode} is currently closed due to war conditions!");

            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest
            };

            context.Result = new BadRequestObjectResult(problemDetails);
        }
    }
}
