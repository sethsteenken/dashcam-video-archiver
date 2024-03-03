namespace DashcamVideoArchive.Core
{
    public interface IDashcam
    {
        Task<bool> IsAvailableAsync();
        Task<IReadOnlyList<DashcamVideoFile>> GetFilesAsync();
        Task DeleteFileAsync(string path);
        Task ShutdownAsync();
    }
}
