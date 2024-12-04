using FlightScanner.Client.BlazorWA.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using System.ComponentModel;

namespace FlightScanner.Client.BlazorWA.Components;

public class AvailableFlightsGridComponentBase : ComponentBase
{
    [Inject]
    public FoundFlightsApplicationState FoundFlightsApplicationState { get; set; } = null!;

    protected PaginationState PaginationModel { get; } = new PaginationState
    {
        ItemsPerPage = 20
    };

    protected string? CurrencyTitle { get; private set; }

    protected string DepartureAirportIataCodeFilter { get; set; } = string.Empty;

    protected IQueryable<FlightOfferViewModel>? FlightOfferVMs => FoundFlightsApplicationState.FlightOfferVMs?
        .Where(i => i.DepartureAirportIataCode.Contains(DepartureAirportIataCodeFilter, StringComparison.OrdinalIgnoreCase))
        .AsQueryable();

    protected override Task OnInitializedAsync()
    {
        FoundFlightsApplicationState.PropertyChanged += FlightsStateChanged;

        CurrencyTitle = FlightOfferVMs?.First().Currency ?? "NaN";

        return base.OnInitializedAsync();
    }

    private void FlightsStateChanged(object? sender, PropertyChangedEventArgs e)
    {
        StateHasChanged();

        CurrencyTitle = FlightOfferVMs?.First().Currency ?? "NaN";
    }
}
