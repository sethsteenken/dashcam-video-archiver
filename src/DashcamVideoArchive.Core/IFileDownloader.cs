namespace DashcamVideoArchive.Core
{
    public interface IFileDownloader
    {
        Task<string> DownloadAsync(string source, string destination, CancellationToken cancellationToken = default);
    }
}
