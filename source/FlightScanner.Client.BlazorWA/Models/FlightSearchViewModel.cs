using System.ComponentModel.DataAnnotations;
using FlightScanner.Common.Constants;
using FlightScanner.Common.Enumerations;

namespace FlightScanner.Client.BlazorWA.Models;

public class FlightSearchViewModel
{
    public Currency SelectedCurrency { get; set; }

    public DateTime DepartureDate { get; set; }

    [Required(ErrorMessage = "Departure airport IATA code is required!")]
    [StringLength(IataCodeConstants.IATA_CODE_LENGTH, ErrorMessage = "IATA code not valid, it must have at least 3 characters!")]
    public string DepartureAirportIataCode { get; set; } = null!;

    [Required(ErrorMessage = "Destionation airport IATA code is required!")]
    [StringLength(IataCodeConstants.IATA_CODE_LENGTH, ErrorMessage = "IATA code not valid, it must have at least 3 characters!")]
    public string DestionationAirportIataCode { get; set; } = null!;

    public DateTime? ReturnDate { get; set; }

    public int NumberOfPassengers { get; set; }
}
