using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FlightScanner.Client.BlazorWA;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using FlightScanner.Client.BlazorWA.Services.Implementations;
using FlightScanner.Client.BlazorWA.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7021"),
    };
    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    return httpClient;
});
builder.Services.AddBlazorBootstrap();

builder.Services.AddSingleton<FoundFlightsApplicationState>();
builder.Services.AddTransient<IAirportsManagerService, AirportsManagerService>();
builder.Services.AddTransient<IFlightSearchService, FlightSearchService>();

await builder.Build().RunAsync();
