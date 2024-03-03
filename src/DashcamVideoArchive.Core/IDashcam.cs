namespace DashcamVideoArchive.Core
{
    public interface IDashcam : IAsyncDisposable
    {
        Task<bool> IsAvailableAsync();
        Task<IReadOnlyList<FootageVideoFile>> GetFilesAsync();
        Task DeleteFileAsync(string path);
    }
}
