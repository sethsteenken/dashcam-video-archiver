using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DashcamVideoArchive.Core
{
    public class DashcamArchiver : IDashcamArchiver
    {
        private readonly IDashcam _dashcam;
        private readonly IFileDownloader _downloader;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DashcamArchiver> _logger;

        public DashcamArchiver(
            IDashcam dashcam, 
            IFileDownloader downloader,
            IConfiguration configuration,
            ILogger<DashcamArchiver> logger)
        {
            _dashcam = dashcam;
            _downloader = downloader;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task ArchiveAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Scanning for dashcam...");

            if (await _dashcam.IsAvailableAsync())
            {
                _logger.LogInformation("Dashcam found. Requesting latest video files...");

                var files = await _dashcam.GetFilesAsync();

                if (files.Count == 0)
                {
                    _logger.LogInformation("No files found.");
                    return;
                }

                _logger.LogInformation($"Downloading files ({files.Count})...");

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    _logger.LogInformation($"Downloading ({i + 1}/{files.Count}) {file.Path}...");
                    var downloadedPath = await _downloader.DownloadAsync(file.Path, cancellationToken);
                    _logger.LogInformation($"Downloaded file to {downloadedPath}.");

                    _logger.LogInformation($"Deleting {file.Path}...");
                    await _dashcam.DeleteFileAsync(file.Path);
                    _logger.LogInformation($"Deleted {file.Path} from dashcam.");
                }
            }
            else
            {
                _logger.LogInformation("Dashcam not available.");
            }

            if (_configuration.GetValue<bool>("SHUTDOWN_ON_COMPLETED"))
            {
                _logger.LogInformation("Forcing dashcam shutdown...");
                await _dashcam.ShutdownAsync();
            }
        }
    }
}
