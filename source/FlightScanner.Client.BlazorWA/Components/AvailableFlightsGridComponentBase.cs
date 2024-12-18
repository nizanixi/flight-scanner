using FlightScanner.Client.BlazorWA.Models;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using FlightScanner.DTOs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace FlightScanner.Client.BlazorWA.Components;

public class AvailableFlightsGridComponentBase : ComponentBase
{
    private const string UNDEFINED_CURRENCY_TITLE = "Unknown currency";

    [Inject]
    public ApplicationState ApplicationState { get; set; } = null!;

    [Inject]
    public IFlightSearchService FlightSearchService { get; set; } = null!;

    [Inject]
    public ProgressBarViewModel ProgressBarVM { get; set; } = null!;

    [Inject]
    public IToastNotificationService ToastNotificationService { get; set; } = null!;

    protected PaginationState PaginationModel { get; } = new PaginationState
    {
        ItemsPerPage = 20
    };

    protected string? CurrencyTitle { get; private set; }

    protected string DepartureAirportIataCodeFilter { get; set; } = string.Empty;

    protected FlightOfferViewModel[]? AllFoundFlightOfferVMs { get; private set; }

    protected IQueryable<FlightOfferViewModel>? FlightOfferVMs => AllFoundFlightOfferVMs?
        .Where(i => i.DepartureAirportIataCode.Contains(DepartureAirportIataCodeFilter, StringComparison.OrdinalIgnoreCase))
        .AsQueryable();

    protected override Task OnInitializedAsync()
    {
        ApplicationState.OnFlightSearchInvoked = OnFlightSearchInvoked;

        CurrencyTitle = FlightOfferVMs?.First().Currency ?? UNDEFINED_CURRENCY_TITLE;

        return base.OnInitializedAsync();
    }

    private async Task OnFlightSearchInvoked(FlightSearchViewModel flightSearchVM)
    {
        AllFoundFlightOfferVMs = null;

        ProgressBarVM.DisplayProgressBar("Searching for flights...");

        IReadOnlyList<FlightEntityDto> flightEntityDtos;
        try
        {
            flightEntityDtos = await FlightSearchService.GetAvailableFlights(flightSearchVM);
        }
        catch (HttpRequestException)
        {
            ToastNotificationService.DisplayErrorNotification(
                title: "Error while searching flights",
                message: "Error while searching flights from external source. Please try again later.");

            return;
        }
        finally
        {
            ProgressBarVM.HideProgressBar();
        }

        AllFoundFlightOfferVMs = flightEntityDtos
            .Select(i => new FlightOfferViewModel(
                departureAirportIataCode: i.DepartureAirportIataCode,
                departureAirportLocation: i.DepartureAirportLocation,
                departureDate: i.DepartureDate,
                arrivalAirportIataCode: i.ArrivalAirportIataCode,
                arrivalAirportLocation: i.ArrivalAirportLocation,
                returnDate: i.ReturnDate,
                numberOfBookableSeats: i.NumberOfBookableSeats,
                numberOfStops: i.NumberOfStops,
                currency: i.Currency,
                price: i.Price))
            .ToArray();

        CurrencyTitle = AllFoundFlightOfferVMs?.FirstOrDefault()?.Currency ?? UNDEFINED_CURRENCY_TITLE;

        StateHasChanged();
    }
}
