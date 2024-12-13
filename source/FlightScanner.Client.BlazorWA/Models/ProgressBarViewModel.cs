namespace FlightScanner.Client.BlazorWA.Models;

public class ProgressBarViewModel
{
    public ProgressBarViewModel()
    {
        Text = string.Empty;
    }

    #region Properties

    public bool IsProgressBarVisible { get; private set; }

    public string Text { get; private set; }

    public Action? ProgressBarStateHasChanged { get; set; }

    #endregion

    #region Public methods

    public void DisplayProgressBar(string text)
    {
        Text = text;
        IsProgressBarVisible = true;

        ProgressBarStateHasChanged?.Invoke();
    }

    public void HideProgressBar()
    {
        Text = string.Empty;
        IsProgressBarVisible = false;

        ProgressBarStateHasChanged?.Invoke();
    }

    #endregion
}
