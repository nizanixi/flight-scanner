using FlightScanner.Client.BlazorWA.Models;
using Microsoft.AspNetCore.Components;

namespace FlightScanner.Client.BlazorWA.Components;

public class FlightSearchComponentBase : ComponentBase
{
    [SupplyParameterFromForm]
    protected FlightSearchData FlightSearchData { get; set; } = null!;

    protected string[] Currencies { get; private set; } = null!;

    protected List<string> DropdownOptions { get; } = Enumerable.Range(1, 700)
        .Select(x => $"Option {x}")
        .ToList();

    protected string MinimumDepartureDate { get; } = DateTime.Today.ToString("yyyy-MM-dd");

    protected string MaximumDepartureDate { get; } = DateTime.Today.AddYears(1).ToString("yyyy-MM-dd");

    protected string MinimumReturnDate { get; private set; } = null!;

    protected string MaximumReturnDate { get; private set; } = null!;

    protected override void OnInitialized()
    {
        FlightSearchData = new FlightSearchData()
        {
            SelectedCurrency = "HRK",
            DepartureDate = DateTime.Now
        };

        MinimumReturnDate = FlightSearchData.DepartureDate.AddHours(2).ToString("yyyy-MM-dd");
        MaximumReturnDate = FlightSearchData.DepartureDate.AddYears(1).ToString("yyyy-MM-dd");

        Currencies = new[]
        {
            "USD",
            "EUR",
            "HRK"
        };
    }

    protected void SubmitForm()
    {

    }
}
