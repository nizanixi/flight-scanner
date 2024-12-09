using BlazorBootstrap;
using FlightScanner.Client.BlazorWA.Services.Contracts;

namespace FlightScanner.Client.BlazorWA.Services.Implementations;

public class ToastNotificationService : IToastNotificationService
{
    public ToastNotificationService()
    {
        ToastMessages = new List<ToastMessage>();
    }

    public List<ToastMessage> ToastMessages { get; }

    public void DisplayErrorNotification(string title, string message)
    {
        var toastMessage = new ToastMessage
        {
            Type = ToastType.Warning,
            Title = title,
            HelpText = $"{DateTime.Now}",
            Message = message,
            AutoHide = false
        };

        ToastMessages.Add(toastMessage);
    }
}
