// See https://aka.ms/new-console-template for more information
using DashcamVideoArchive.Core;
using DashcamVideoArchive.Viofo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient("viofo", (serviceProvider, httpClient) =>
        {
            var endpoint = serviceProvider.GetRequiredService<IConfiguration>()["DASHCAM_ENDPOINT"]
                ?? throw new InvalidOperationException("Missing DASHCAM_ENDPOINT value.");

            httpClient.BaseAddress = new Uri(endpoint);
        });

        services.AddTransient<IDashcam, ViofoASeriesDashcam>(serviceProvider =>
        {
            return new ViofoASeriesDashcam(
                serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("viofo"));
        });
    })
    .Build();

var dashcam = host.Services.GetRequiredService<IDashcam>();

if (await dashcam.IsAvailableAsync())
{
    var files = await dashcam.GetFilesAsync();

    foreach (var file in files)
    {
        Console.WriteLine(file.Name);
        Console.WriteLine($" - {file.Path}");
        Console.WriteLine($" - {file.Size}");
        Console.WriteLine($" - {file.Time}");
    }
}
else
{
    Console.WriteLine("Dashcam is not available");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
