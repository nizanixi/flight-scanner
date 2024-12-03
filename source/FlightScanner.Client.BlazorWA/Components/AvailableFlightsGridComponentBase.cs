using FlightScanner.Client.BlazorWA.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace FlightScanner.Client.BlazorWA.Components;

public class AvailableFlightsGridComponentBase : ComponentBase
{
    protected PaginationState PaginationModel { get; } = new PaginationState
    {
        ItemsPerPage = 20
    };

    protected string? CurrencyTitle { get; private set; }

    protected string DepartureAirportIataCodeFilter { get; set; } = string.Empty;

    protected IQueryable<FlightOffer> FlightOffers => Enumerable.Range(1, 100)
        .Select(i =>
        {
            return new FlightOffer("AAA", DateTime.Now, "CCC", DateTime.Now, (uint)i, i, "EUR", 42);
        })
        .Where(i => i.DepartureAirportIataCode.Contains(DepartureAirportIataCodeFilter, StringComparison.OrdinalIgnoreCase))
        .AsQueryable();

    protected override Task OnInitializedAsync()
    {
        CurrencyTitle = "Euro";

        return base.OnInitializedAsync();
    }
}
