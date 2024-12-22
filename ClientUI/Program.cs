using APPL.DepandecyInjection;
using Blazored.LocalStorage;
using ClientUI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");


builder.RootComponents.Add<HeadOutlet>("head::after");
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

//  ”ÃÌ· AppSettings ﬂŒœ„… ﬁ«»·… ··Õﬁ‰
var appSettings = configuration.Get<AppSettings>();
builder.Services.AddSingleton(appSettings);
builder.Services.AddSingleton<ApiService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddApplicationService();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
