// See https://aka.ms/new-console-template for more information
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
            httpClient.BaseAddress = new Uri("https://dashcam.steenkenparrett.house");
        });

        services.AddTransient<ViofoASeriesDashcam>(serviceProvider =>
        {
            return new ViofoASeriesDashcam(
                serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("viofo"));
        });
    })
    .Build();

var dashcam = host.Services.GetRequiredService<ViofoASeriesDashcam>();

if (await dashcam.IsAvailableAsync())
{
    await dashcam.PrintFilesAsync();
}
else
{
    Console.WriteLine("Dashcam is not available");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
