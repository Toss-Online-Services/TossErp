using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TossErp.WebApp;
using TossErp.WebApp.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add MudBlazor
builder.Services.AddMudServices();

// Add Authentication Service
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Local Storage Service
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

await builder.Build().RunAsync(); 
