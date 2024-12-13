using FlightScanner.Client.BlazorWA.Models;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using FlightScanner.Common.Enumerations;
using FlightScanner.DTOs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace FlightScanner.Client.BlazorWA.Components;

public class FlightSearchComponentBase : ComponentBase
{
    private const string WEB_CLIENT_DATE_TIME_FORMAT = "yyyy-MM-dd";
    private const string DEFAULT_VALUE_FOR_UNSELECTED_AIRPORT_LOCATION = "Select an option";

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

    protected string DepartureAirportSearch { get; set; } = string.Empty;

    protected string DestinationAirportSearch { get; set; } = string.Empty;

    protected bool IsDepartureAirportDropdownOpen { get; set; }

    protected bool IsDestinationAirportDropdownOpen { get; set; }

    protected string SelectedDepartureAirportName { get; set; } = DEFAULT_VALUE_FOR_UNSELECTED_AIRPORT_LOCATION;

    protected string SelectedDestinationAirportName { get; set; } = DEFAULT_VALUE_FOR_UNSELECTED_AIRPORT_LOCATION;

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
    }

    protected ValueTask<ItemsProviderResult<AirportDto>> LoadDepartureAirportsToVirtualizationControl(ItemsProviderRequest request)
    {
        if (AllAvailableAirports == null)
        {
            return ValueTask.FromResult(new ItemsProviderResult<AirportDto>([], default));
        }

        var itemsToBeDisplayed = AllAvailableAirports
            .Where(airport => string.IsNullOrEmpty(DepartureAirportSearch) ||
                airport.Location.Contains(DepartureAirportSearch, StringComparison.OrdinalIgnoreCase))
            .Skip(request.StartIndex)
            .Take(request.Count);

        var itemsProviderResult = new ItemsProviderResult<AirportDto>(
            items: itemsToBeDisplayed,
            totalItemCount: AllAvailableAirports.Length);

        return ValueTask.FromResult(itemsProviderResult);
    }

    protected ValueTask<ItemsProviderResult<AirportDto>> LoadDestinationAirportsToVirtualizationControl(ItemsProviderRequest request)
    {
        if (AllAvailableAirports == null)
        {
            return ValueTask.FromResult(new ItemsProviderResult<AirportDto>([], default));
        }

        var itemsToBeDisplayed = AllAvailableAirports
            .Where(airport => string.IsNullOrEmpty(DestinationAirportSearch) ||
                airport.Location.Contains(DestinationAirportSearch, StringComparison.OrdinalIgnoreCase))
            .Skip(request.StartIndex)
            .Take(request.Count);

        var itemsProviderResult = new ItemsProviderResult<AirportDto>(
            items: itemsToBeDisplayed,
            totalItemCount: AllAvailableAirports.Length);

        return ValueTask.FromResult(itemsProviderResult);
    }

    protected void ToggleDepartureAirportDropdown()
    {
        IsDepartureAirportDropdownOpen = !IsDepartureAirportDropdownOpen;

        if (!IsDepartureAirportDropdownOpen)
        {
            DepartureAirportSearch = string.Empty;
        }
    }

    protected void ToggleDestinationAirportDropdown()
    {
        IsDestinationAirportDropdownOpen = !IsDestinationAirportDropdownOpen;

        if (!IsDestinationAirportDropdownOpen)
        {
            DestinationAirportSearch = string.Empty;
        }
    }

    protected void SelectDepartureAirport(AirportDto airport)
    {
        FlightSearchVM.DepartureAirportIataCode = airport.IataCode;
        SelectedDepartureAirportName = airport.Location;
        IsDepartureAirportDropdownOpen = false;
        DepartureAirportSearch = string.Empty;
    }

    protected void SelectDestinationAirport(AirportDto airport)
    {
        FlightSearchVM.DestionationAirportIataCode = airport.IataCode;
        SelectedDestinationAirportName = airport.Location;
        IsDestinationAirportDropdownOpen = false;
        DestinationAirportSearch = string.Empty;
    }
}
