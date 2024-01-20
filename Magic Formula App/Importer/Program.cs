using Microsoft.Net.Http.Headers;
using System.Net;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Importer.Worker>();
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

var host = builder.Build();
host.Run();
