namespace FlightScanner.Client.BlazorWA.Models;

public class ApplicationState
{
    public Func<FlightSearchViewModel, Task> OnFlightSearchInvoked { get; set; } = null!;
}
