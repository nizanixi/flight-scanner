using System.ComponentModel.DataAnnotations;
using FlightScanner.Client.BlazorWA.Validation;
using FlightScanner.Common.Enumerations;

namespace FlightScanner.Client.BlazorWA.Models;

public class FlightSearchViewModel
{
    [Required(ErrorMessage = "Currency is required!")]
    public Currency SelectedCurrency { get; set; }

    [Required(ErrorMessage = "Departure date is required!")]
    public DateTime DepartureDate { get; set; }

    [Required(ErrorMessage = "Departure airport IATA code is required!")]
    [StringLength(3, ErrorMessage = "IATA code not valid, it must have at least 3 characters!")]
    public string DepartureAirportIataCode { get; set; } = null!;

    [Required(ErrorMessage = "Destionation airport IATA code is required!")]
    [StringLength(3, ErrorMessage = "IATA code not valid, it must have at least 3 characters!")]
    public string DestionationAirportIataCode { get; set; } = null!;

    [ReturnDateValidation(nameof(DepartureDate))]
    public DateTime? ReturnDate { get; set; }

    [Required(ErrorMessage = "Number of passengers is required!")]
    [Range(1, 9)]
    public int NumberOfPassengers { get; set; }
}
