using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DashcamVideoArchive.Core
{
    public class DashcamArchiver : IDashcamArchiver
    {
        private readonly IFileDownloader _downloader;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DashcamArchiver> _logger;

        public DashcamArchiver(
            IFileDownloader downloader,
            IConfiguration configuration,
            ILogger<DashcamArchiver> logger)
        {
            _downloader = downloader;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task ArchiveAsync(IDashcam dashcam, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Scanning for dashcam...");

            if (!await dashcam.IsAvailableAsync())
            {
                _logger.LogInformation("Dashcam not found.");
                return;
            }

            _logger.LogInformation("Dashcam found. Requesting latest video files...");

            var files = await dashcam.GetFilesAsync();

            if (files.Count == 0)
            {
                _logger.LogInformation("No files found.");
                return;
            }

            string currentRide;
            int rideCount = 0;

            string rideDateDirectoryFormat = _configuration.GetRideDateDirectoryFormat();
            int rideTimeSeparation = _configuration.GetRideTimeSeparation();

            void setNewRide(DashcamVideoFile file)
            {
                currentRide = file.Time.ToString(rideDateDirectoryFormat);
                rideCount++;
                _logger.LogInformation($"New ride ({rideCount}) detected: {currentRide}.");
            }

            // first file will be considered a new ride
            setNewRide(files[0]);

            _logger.LogInformation($"Downloading files ({files.Count})...");

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];

                // if the time difference between the previous file and the current file is greater than n minutes
                // assume it's a new ride
                if (i > 0 && file.Time.Subtract(files[i - 1].Time).TotalMinutes > rideTimeSeparation)
                    setNewRide(file);

                string storagePath = file.GetStoragePath(currentRide);

                // always download files (normal recordings and event recordings) if not already downloaded
                // or if it's not an event recording, download file to be sure it's the latest full recording
                if (!file.IsEventRecording || !File.Exists(storagePath))
                {
                    _logger.LogInformation($"Downloading ({i + 1}/{files.Count}) {file.Path}...");
                    var downloadedPath = await _downloader.DownloadAsync(file.Path, storagePath, cancellationToken);
                    _logger.LogInformation($"Downloaded file to {downloadedPath}.");

                    if (!file.IsEventRecording)
                    {
                        _logger.LogInformation($"Deleting {file.Path}...");
                        await dashcam.DeleteFileAsync(file.Path);
                        _logger.LogInformation($"Deleted {file.Path} from dashcam.");
                    }
                    else
                        _logger.LogWarning($"File {file.Path} is an event recording and cannot be deleted from dashcam.");
                }
            }

            if (_configuration.ShutdownOnCompleted())
            {
                _logger.LogInformation("Forcing dashcam shutdown...");
                await dashcam.ShutdownAsync();
            }
        }
    }
}
