namespace DashcamVideoArchive.Core
{
    public interface IDashcam
    {
        Task<bool> IsAvailableAsync();
        Task<IReadOnlyList<FootageVideoFile>> GetFilesAsync();
        Task DeleteFileAsync(string path);
        Task ShutdownAsync();
    }
}
