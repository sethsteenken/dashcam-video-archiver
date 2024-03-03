// See https://aka.ms/new-console-template for more information
using DashcamVideoArchive.Core;
using DashcamVideoArchive.Viofo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#if DEBUG
Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
#endif

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((hostContext, builder) =>
    {
        builder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = false;
            options.SingleLine = true;
            options.TimestampFormat = "dd-MM-yyyy HH:mm:ss ";
        });
        
        if (!hostContext.HostingEnvironment.IsDevelopment())
        {
            builder.AddFilter("Microsoft*", logLevel => logLevel >= LogLevel.Warning);
            builder.AddFilter("System.Net.Http.HttpClient*", logLevel => logLevel >= LogLevel.Warning);
        }
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.ConfigureHttpClientDefaults(builder =>
        {
            builder.ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                var endpoint = serviceProvider.GetRequiredService<IConfiguration>()["DASHCAM_ENDPOINT"]
                ?? throw new InvalidOperationException("Missing DASHCAM_ENDPOINT value.");

                httpClient.BaseAddress = new Uri(endpoint);
            });
        });

        services.AddTransient<IDashcam, ViofoASeriesDashcam>();
        services.AddTransient<IFileDownloader, FileDownloader>();
    })
    .Build();


var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    await using var dashcam = host.Services.GetRequiredService<IDashcam>();

    logger.LogInformation("Scanning for dashcam...");

    if (await dashcam.IsAvailableAsync())
    {
        logger.LogInformation("Dashcam found. Requesting latest video files...");

        var files = await dashcam.GetFilesAsync();

        if (files.Count == 0)
        {
            logger.LogInformation("No files found.");
            return 0;
        }

        logger.LogInformation($"Downloading files ({files.Count})...");

        var cancellationTokenSource = new CancellationTokenSource();
        var downloader = host.Services.GetRequiredService<IFileDownloader>();

        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            logger.LogInformation($"Downloading ({i + 1}/{files.Count}) {file.Path}...");
            var downloadedPath = await downloader.DownloadAsync(file.Path, cancellationTokenSource.Token);
            logger.LogInformation($"Downloaded file to {downloadedPath}.");

            logger.LogInformation($"Deleting {file.Path}...");
            await dashcam.DeleteFileAsync(file.Path);
            logger.LogInformation($"Deleted {file.Path} from dashcam.");
        }
    }
    else
    {
        logger.LogInformation("Dashcam not available.");
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while scanning the dashcam.");
    return 1;
}
finally
{
#if DEBUG
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
#endif
}

return 0;

