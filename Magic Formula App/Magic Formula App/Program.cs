using Microsoft.Net.Http.Headers;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient("SecCompanyTickers", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://www.sec.gov/");
    httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, "gzip, deflate");
    httpClient.DefaultRequestHeaders.Add(HeaderNames.Host, "www.sec.gov");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.UserAgent, "Lekasoft aldo@lekasoft.com");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
});

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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
