using FlightScanner.Client.BlazorWA.Models;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using FlightScanner.Common.Enumerations;
using FlightScanner.DTOs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FlightScanner.Client.BlazorWA.Components;

public class FlightSearchComponentBase : ComponentBase
{
    private const string WEB_CLIENT_DATE_TIME_FORMAT = "yyyy-MM-dd";

    [Inject]
    public IAirportsManagerService AirportsManagerService { get; set; } = null!;

    [Inject]
    public IFlightSearchService FlightSearchService { get; set; } = null!;

    [Inject]
    public FoundFlightsApplicationState FoundFlightsApplicationState { get; set; } = null!;

    [Inject]
    public IToastNotificationService ToastNotificationService { get; set; } = null!;

    [SupplyParameterFromForm]
    protected FlightSearchViewModel FlightSearchVM { get; set; } = null!;

    protected Currency[] Currencies { get; } = Enum.GetValues<Currency>();

    protected AirportDto[]? AllAvailableAirports { get; private set; }

    protected string MinimumDepartureDate { get; } = DateTime.Today.ToString(WEB_CLIENT_DATE_TIME_FORMAT);

    protected string MaximumDepartureDate { get; } = DateTime.Today.AddYears(1).ToString(WEB_CLIENT_DATE_TIME_FORMAT);

    protected string MinimumReturnDate => FlightSearchVM.DepartureDate.AddHours(2).ToString(WEB_CLIENT_DATE_TIME_FORMAT);

    protected string MaximumReturnDate => FlightSearchVM.DepartureDate.AddYears(1).ToString(WEB_CLIENT_DATE_TIME_FORMAT);

    protected override async Task OnInitializedAsync()
    {
        FlightSearchVM = new FlightSearchViewModel()
        {
            SelectedCurrency = Currency.EUR,
            DepartureDate = DateTime.Now.AddDays(1),
            NumberOfPassengers = 1,
            ReturnDate = DateTime.Now.AddDays(7),
        };

        try
        {
            var foundAirportDtos = await AirportsManagerService.GetAllAirports();
            AllAvailableAirports = foundAirportDtos
                .OrderBy(i => i.Location)
                .ToArray();
        }
        catch (HttpRequestException)
        {
            ToastNotificationService.DisplayErrorNotification(
                title: "Error while getting airports",
                message: "Error while getting IATA airport codes from database. " +
                "No airport codes found or connection to database not established. Please try again later.");
        }
    }

    protected async Task SubmitValidForm()
    {
        FoundFlightsApplicationState.FlightOfferVMs = null;
        FoundFlightsApplicationState.IsSearchingOfFlightsInProgress = true;

        IReadOnlyList<FlightEntityDto> flightEntityDtos;
        try
        {
            flightEntityDtos = await FlightSearchService.GetAvailableFlights(FlightSearchVM);
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
            FoundFlightsApplicationState.IsSearchingOfFlightsInProgress = false;
        }

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
