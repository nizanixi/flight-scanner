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

    private bool _isSearchingOfFlightsInProgress;
    public bool IsSearchingOfFlightsInProgress
    {
        get => _isSearchingOfFlightsInProgress;
        internal set
        {
            _isSearchingOfFlightsInProgress = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSearchingOfFlightsInProgress)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
