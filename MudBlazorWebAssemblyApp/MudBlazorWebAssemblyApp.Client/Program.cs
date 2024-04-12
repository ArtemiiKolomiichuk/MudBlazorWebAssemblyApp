using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddScoped<MudBlazorWebAssemblyApp.Client.LocalStorageService>();
builder.Services.AddScoped<MudBlazorWebAssemblyApp.Client.AuthService>();

await builder.Build().RunAsync();
