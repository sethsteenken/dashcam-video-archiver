namespace DashcamVideoArchive.Core
{
    public interface IFileDownloader
    {
        Task<string> DownloadAsync(string path, CancellationToken cancellationToken = default);
    }
}
