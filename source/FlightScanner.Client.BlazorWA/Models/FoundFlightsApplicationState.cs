using System.ComponentModel;

namespace FlightScanner.Client.BlazorWA.Models;

public class FoundFlightsApplicationState : INotifyPropertyChanged
{
    private IReadOnlyList<FlightOfferViewModel>? _flightOfferVMs;
    public IReadOnlyList<FlightOfferViewModel>? FlightOfferVMs
    {
        get => _flightOfferVMs;
        set
        {
            _flightOfferVMs = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FlightOfferVMs)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
