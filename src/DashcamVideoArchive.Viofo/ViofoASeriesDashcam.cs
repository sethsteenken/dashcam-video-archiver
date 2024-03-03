using DashcamVideoArchive.Core;
using DashcamVideoArchive.Viofo.Xml;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace DashcamVideoArchive.Viofo
{
    public class ViofoASeriesDashcam : IDashcam
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ViofoASeriesDashcam> _logger;

        public ViofoASeriesDashcam(
            HttpClient httpClient,
            ILogger<ViofoASeriesDashcam> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                _logger.LogInformation("Checking dashcam heartbeat...");
                var response = await _httpClient.SendCommandAsync(CommandCodes.Heartbeat);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException hrex)
            {
                _logger.LogDebug(hrex, "An error occurred while checking dashcam heartbeat.");
                return false;
            }
        }

        public async Task<IReadOnlyList<DashcamVideoFile>> GetFilesAsync()
        {
            var response = await _httpClient.SendCommandAsync(CommandCodes.GetFiles);

            response.EnsureSuccessStatusCode();

            string xml = await response.Content.ReadAsStringAsync();

            var serializer = new XmlSerializer(typeof(FileList));

            var result = serializer.Deserialize<FileList>(xml);
            if (result is null)
                throw new InvalidOperationException("Failed to deserialize XML.");

            return result.AllFiles.Select(f => f.File)
                                  .Where(f => !f.FilePath?.Contains("\\RO") ?? false)
                                  .Select(f => new DashcamVideoFile()
                                  {
                                      Name = f.Name ?? throw new NullReferenceException("Name is required."),
                                      Path = FormatFilePath(f.FilePath ?? throw new NullReferenceException("FilePath is required.")),
                                      Time = DateTime.Parse(f.Time),
                                      Size = f.Size
                                  })
                                  .OrderBy(f => f.Time)
                                  .ToList();
        }

        public async Task DeleteFileAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var response = await _httpClient.GetAsync($"{path}?del=1");

            response.EnsureSuccessStatusCode();
        }

        private static string FormatFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            return Path.GetFullPath(path).Substring(Path.GetPathRoot(path)?.Length ?? 0).Replace("\\", "/");
        }

        public async Task ShutdownAsync()
        {
            var response = await _httpClient.SendCommandAsync(CommandCodes.ForcePowerOff);
            response.EnsureSuccessStatusCode();
        }
    }
}
