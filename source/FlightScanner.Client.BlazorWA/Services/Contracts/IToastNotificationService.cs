using BlazorBootstrap;

namespace FlightScanner.Client.BlazorWA.Services.Contracts;

public interface IToastNotificationService
{
    List<ToastMessage> ToastMessages { get; }

    void DisplayErrorNotification(string title, string message);
}
