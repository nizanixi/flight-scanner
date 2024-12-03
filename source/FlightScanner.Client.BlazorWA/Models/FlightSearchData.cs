using System.ComponentModel.DataAnnotations;

namespace FlightScanner.Client.BlazorWA.Models;

public class FlightSearchData
{
    [Required(ErrorMessage = "Currency is required!")]
    public string SelectedCurrency { get; set; } = null!;

    [Required(ErrorMessage = "Departure date is required!")]
    public DateTime DepartureDate { get; set; }

    [Required(ErrorMessage = "Departure airport IATA code is required!")]
    public string DepartureAirportIataCode { get; set; } = null!;

    [Required(ErrorMessage = "Destionation airport IATA code is required!")]
    public string DestionationAirportIataCode { get; set; } = null!;

    public DateTime? ReturnDate { get; set; }

    [Required(ErrorMessage = "Number of passengers is required!")]
    [Range(0, 9)]
    public int NumberOfPassengers { get; set; }
}
