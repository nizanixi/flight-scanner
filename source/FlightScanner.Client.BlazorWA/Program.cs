using FlightScanner.Client.BlazorWA;
using FlightScanner.Client.BlazorWA.Configurations;
using FlightScanner.Client.BlazorWA.Models;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using FlightScanner.Client.BlazorWA.Services.Implementations;
using FlightScanner.Common.Constants;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<IBlazorWAConfiguration, BlazorWAConfiguration>();
builder.Services.AddScoped(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IBlazorWAConfiguration>();

    var httpClient = new HttpClient
    {
        BaseAddress = new Uri(configuration.BackendApiBaseUri),
    };
    httpClient.DefaultRequestHeaders.Add(HttpHeaderConstants.ACCEPT_TYPE, MimeTypeConstants.JSON);
    return httpClient;
});
builder.Services.AddBlazorBootstrap();

builder.Services.AddSingleton<ApplicationState>();
builder.Services.AddTransient<IAirportsManagerService, AirportsManagerService>();
builder.Services.AddTransient<IFlightSearchService, FlightSearchService>();
builder.Services.AddSingleton<IToastNotificationService, ToastNotificationService>();
builder.Services.AddSingleton<ProgressBarViewModel>();

await builder.Build().RunAsync();
