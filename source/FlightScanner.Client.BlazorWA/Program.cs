using FlightScanner.Client.BlazorWA;
using FlightScanner.Client.BlazorWA.Configurations;
using FlightScanner.Client.BlazorWA.Constants;
using FlightScanner.Client.BlazorWA.Models;
using FlightScanner.Client.BlazorWA.Services.Contracts;
using FlightScanner.Client.BlazorWA.Services.Implementations;
using FlightScanner.Client.BlazorWA.Validation;
using FlightScanner.Common.Constants;
using FlightScanner.Common.Policies;
using FluentValidation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<IBlazorWAConfiguration, BlazorWAConfiguration>();
builder.Services.AddHttpClient(HttpConstants.BACKEND_HTTP_CLIENT_NAME, (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IBlazorWAConfiguration>();

    client.BaseAddress = new Uri(configuration.BackendApiBaseUri);
    client.DefaultRequestHeaders.Add(HttpHeaderConstants.ACCEPT_TYPE, MimeTypeConstants.JSON);
    client.Timeout = TimeSpan.FromSeconds(HttpConstants.HTTP_TIMEOUT_IN_SECONDS);
})
.AddPolicyHandler(HttpPoliciesFactory.CreateRetryPolicyWithSameRetryTime(HttpConstants.HTTP_RETRY_IN_SECONDS));
builder.Services.AddBlazorBootstrap();

builder.Services.AddValidatorsFromAssemblyContaining<FlightSearchViewModelValidator>();

builder.Services.AddSingleton<ApplicationState>();
builder.Services.AddTransient<IAirportsManagerService, AirportsManagerService>();
builder.Services.AddTransient<IFlightSearchService, FlightSearchService>();
builder.Services.AddSingleton<IToastNotificationService, ToastNotificationService>();
builder.Services.AddSingleton<ProgressBarViewModel>();

await builder.Build().RunAsync();
