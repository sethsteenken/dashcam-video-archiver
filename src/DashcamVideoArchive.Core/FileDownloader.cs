﻿using Microsoft.Extensions.Configuration;

namespace DashcamVideoArchive.Core
{
    public class FileDownloader : IFileDownloader
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public FileDownloader(
            HttpClient client, 
            IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task<string> DownloadAsync(string path, CancellationToken cancellationToken = default)
        {
            string downloadDirectory = _configuration["DOWNLOAD_DIRECTORY"] ?? throw new InvalidOperationException("Missing DOWNLOAD_DIRECTORY value.");
            string downloadPath = Path.Combine(downloadDirectory, path);
            Directory.CreateDirectory(Path.GetDirectoryName(downloadPath) ?? string.Empty);

            using var progress = new ConsoleProgressIndicator();
            using var response = await _client.GetAsync(path, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            
            response.EnsureSuccessStatusCode();

            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(downloadPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 8192, true);
                
            var totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1L;
            var totalBytesRead = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;

            do
            {
                var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                if (bytesRead == 0)
                {
                    isMoreToRead = false;
                }
                else
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);

                    totalBytesRead += bytesRead;
                    if (totalBytes != -1)
                    {
                        var percentage = totalBytes > 0 ? (totalBytesRead * 1d) / (totalBytes * 1d) : 0;
                        progress.Report(percentage);
                    }
                }
            } while (isMoreToRead);

            return downloadPath;
        }
    }
}