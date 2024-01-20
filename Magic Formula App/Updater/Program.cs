using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Shared.Models;
using System.Net;
using Updater;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient("SecClient", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["BaseAddress"]);
    httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptEncoding, builder.Configuration["AcceptEncoding"]);
    httpClient.DefaultRequestHeaders.Add(HeaderNames.Host, builder.Configuration["Host"]);
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.UserAgent, builder.Configuration["UserAgent"]);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
});

builder.Services.AddDbContext<CompanyData>(options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

var host = builder.Build();
host.Run();
