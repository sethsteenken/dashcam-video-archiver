namespace DashcamVideoArchive.Core
{
    public interface IDashcamArchiver
    {
        Task ArchiveAsync(CancellationToken cancellationToken = default);
    }
}
