using IdleFactory;
using IdleFactory.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<FactoryDataService>();
builder.Services.AddSingleton<GameLogicService>();

await builder.Build().RunAsync();
