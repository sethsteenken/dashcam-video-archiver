namespace DashcamVideoArchive.Core
{
    public interface IDashcam
    {
        Task<bool> IsAvailableAsync();
        Task StopRecordingAsync();
    }
}
