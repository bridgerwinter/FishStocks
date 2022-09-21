using FishStocks.Website;
using FishStocks.Website.Data.FishStocks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
 string ConnectionString = @"Data Source=SUNBEAR\SQLEXPRESS;Initial Catalog = FishStocks; Integrated Security = True";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//builder.Services.AddAntDesign();
var x = builder.Configuration.GetSection("iisSettings");
builder.Services.AddDbContext<FishStocksContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<FishStocksService>();
await builder.Build().RunAsync();
