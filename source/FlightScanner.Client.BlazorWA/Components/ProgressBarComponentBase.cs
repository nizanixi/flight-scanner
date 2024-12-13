using FlightScanner.Client.BlazorWA.Models;
using Microsoft.AspNetCore.Components;

namespace FlightScanner.Client.BlazorWA.Components;

public class ProgressBarComponentBase : ComponentBase
{
    [Inject]
    public ProgressBarViewModel ProgressBarVM { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        ProgressBarVM.ProgressBarStateHasChanged = StateHasChanged;

        return base.OnInitializedAsync();
    }
}
