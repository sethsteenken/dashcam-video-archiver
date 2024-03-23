using DashcamVideoArchive.Core;
using DashcamVideoArchive.Viofo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

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
        services.AddDefaultDashcamServices();
        services.AddTransient<IDashcam, ViofoASeriesDashcam>();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    var cancellationTokenSource = new CancellationTokenSource();
    var dashcam = host.Services.GetRequiredService<IDashcam>();
    var archiver = host.Services.GetRequiredService<IDashcamArchiver>();
    await archiver.ArchiveAsync(dashcam, cancellationTokenSource.Token);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred scanning or downloading files from dashcam.");
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

