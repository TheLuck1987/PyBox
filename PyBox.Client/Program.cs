using PyBox.Shared.Services.Classes;
using PyBox.Shared.Services.Interfaces;
using PyBox.Shared.Services.Utilities;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var baseAddress = builder.Configuration.GetValue<string>("BaseApiUrl") == null ? "https://localhost/" : builder.Configuration.GetValue<string>("BaseApiUrl");
builder.Services.AddScoped(scoped => new HttpClient { BaseAddress = new Uri(baseAddress!) });
builder.Services.AddScoped<HttpHelper>();
builder.Services.AddScoped<IScriptDataService, HttpScriptDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
