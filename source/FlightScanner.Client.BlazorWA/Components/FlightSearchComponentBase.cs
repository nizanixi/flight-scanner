using FlightScanner.Client.BlazorWA.Models;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FlightScanner.Client.BlazorWA.Components;

public class FlightSearchComponentBase : ComponentBase
{
    [Inject]
    public IAirportsManagerService AirportsManagerService { get; set; } = null!;

    [Inject]
    public IFlightSearchService FlightSearchService { get; set; } = null!;

    [Inject]
    public FoundFlightsApplicationState FoundFlightsApplicationState { get; set; } = null!;

    [SupplyParameterFromForm]
    protected FlightSearchViewModel FlightSearchVM { get; set; } = null!;

    protected EditContext FlightSearchEditContext { get; private set; } = null!;

    protected string[] Currencies { get; private set; } = null!;

    protected string[]? AllAvailableAirportIataCodes { get; private set; }

    protected string MinimumDepartureDate { get; } = DateTime.Today.ToString("yyyy-MM-dd");

    protected string MaximumDepartureDate { get; } = DateTime.Today.AddYears(1).ToString("yyyy-MM-dd");

    protected string MinimumReturnDate { get; private set; } = null!;

    protected string MaximumReturnDate { get; private set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var airportDtos = await AirportsManagerService.GetAllAirports();
        AllAvailableAirportIataCodes = airportDtos
            .Select(i => i.IataCode)
            .ToArray();

        FlightSearchVM = new FlightSearchViewModel()
        {
            SelectedCurrency = "EUR",
            DepartureDate = DateTime.Now.AddDays(1),
            NumberOfPassengers = 1,
            ReturnDate = DateTime.Now.AddDays(7),
        };

        MinimumReturnDate = FlightSearchVM.DepartureDate.AddHours(2).ToString("yyyy-MM-dd");
        MaximumReturnDate = FlightSearchVM.DepartureDate.AddYears(1).ToString("yyyy-MM-dd");

        Currencies = new[]
        {
            "USD",
            "EUR",
            "HRK"
        };

        FlightSearchEditContext = new EditContext(FlightSearchVM);
    }

    protected async Task SubmitForm()
    {
        if (!FlightSearchEditContext.Validate())
        {
            return;
        }

        var flightEntityDtos = await FlightSearchService.GetAvailableFlights(FlightSearchVM);

        FoundFlightsApplicationState.FlightOfferVMs = flightEntityDtos
            .Select(i => new FlightOfferViewModel(
                departureAirportIataCode: i.DepartureAirportIataCode,
                departureDate: i.DepartureDate,
                arrivalAirportIataCode: i.ArrivalAirportIataCode,
                returnDate: i.ReturnDate,
                numberOfBookableSeats: i.NumberOfBookableSeats,
                numberOfStops: i.NumberOfStops,
                currency: i.Currency,
                price: i.Price))
            .ToArray();
    }
}
