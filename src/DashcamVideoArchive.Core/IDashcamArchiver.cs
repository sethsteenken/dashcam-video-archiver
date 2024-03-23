namespace DashcamVideoArchive.Core
{
    public interface IDashcamArchiver
    {
        Task ArchiveAsync(IDashcam dashcam, CancellationToken cancellationToken = default);
    }
}
